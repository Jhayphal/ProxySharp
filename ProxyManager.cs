using ProxySharp.Providers.Selectors;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxySharp
{
    /// <summary>
    /// Provides methods to run requests through proxy by the current selector.
    /// </summary>
    public sealed class ProxyManager : IProxyManager<HttpClient, HttpResponseMessage>
    {
        private readonly IProxySelector _selector;
        private Func<HttpResponseMessage, Task<bool>> _validateAsync;
        private Action<HttpClient> _configure;
        private HttpClient _httpClient;
        private HttpClientHandler _httpClientHandler;

        /// <summary>
        /// Create new instance.
        /// </summary>
        /// <param name="selector">Proxy selector.</param>
        public ProxyManager(IProxySelector selector)
        {
            _selector = selector;
            
            _selector.CurrentChanged += SetWebProxy;

            _selector.SetCurrent();
        }

        /// <summary>
        /// Set next proxy by current selector.
        /// </summary>
        /// <returns>This instance.</returns>
        public IProxyManager<HttpClient, HttpResponseMessage> ChangeProxy()
        {
            _selector.SetCurrentToNext();

            return this;
        }

        /// <summary>
        /// Set validator to check query response.
        /// </summary>
        /// <param name="func">Validator.</param>
        /// <returns>This instance.</returns>
        public IProxyManager<HttpClient, HttpResponseMessage> UseValidator(Func<HttpResponseMessage, Task<bool>> func)
        {
            _validateAsync = func;

            return this;
        }

        /// <summary>
        /// Configure HttpClient used by the query.
        /// </summary>
        /// <param name="func">Setup function.</param>
        /// <returns>This instance.</returns>
        public IProxyManager<HttpClient, HttpResponseMessage> Configure(Action<HttpClient> func)
        {
            _configure = func;

            CreateAndConfigureClient();

            return this;
        }

        /// <summary>
        /// Run request to get the result.
        /// </summary>
        /// <remarks>
        /// If the request fails or the validator return <c>False</c>, will try to run the request through the next proxy.
        /// </remarks>
        /// <param name="func">Request.</param>
        /// <returns>Response.</returns>
        /// <exception cref="InvalidOperationException">Has no valid proxies.</exception>
        /// <exception cref="ArgumentNullException">Has no HTTP client.</exception>
        public async Task<HttpResponseMessage> RequestAsync(Func<HttpClient, Task<HttpResponseMessage>> func)
        {
            while (true)
            {
                ++_selector.Current.UsedCount;

                HttpResponseMessage response;

                try
                {
                    if (_httpClient == null)
                        throw new ArgumentNullException(nameof(_httpClient));

                    response = await func(_httpClient);
                }
                catch (Exception error)
                {
                    error.LogError();

                    ++_selector.Current.FailsCount;
                    _selector.Current.LastException = error;

                    _selector.SetCurrentToNext();

                    continue;
                }

                if (_validateAsync == null)
                    return response;

                try
                {
                    if (!await _validateAsync(response))
                    {
                        ++_selector.Current.FailsCount;

                        _selector.SetCurrentToNext();

                        continue;
                    }
                }
                catch (Exception error)
                {
                    error.LogError();

                    throw;
                }

                return response;
            }
        }

        /// <summary>
        /// Set current proxy.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">New proxy.</param>
        private void SetWebProxy(object sender, ProxyInfo e)
        {
            _httpClientHandler = new HttpClientHandler
            {
                Proxy = e.ToWebProxy()
            };

            CreateAndConfigureClient();
        }

        /// <summary>
        /// Creates and configures an HTTP client.
        /// </summary>
        /// <exception cref="ArgumentNullException">Fires when the proxy is not set.</exception>
        private void CreateAndConfigureClient()
        {
            if (_httpClientHandler == null)
                SetWebProxy(_selector, _selector.Current);

            _httpClient?.Dispose();

            _httpClient = new HttpClient(_httpClientHandler, disposeHandler: true);

            _configure?.Invoke(_httpClient);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}