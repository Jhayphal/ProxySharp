using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxySharp
{
    public interface IProxyManager<TClient, TResponse> : IDisposable
    {
        IProxyManager<TClient, TResponse> ChangeProxy();

        IProxyManager<TClient, TResponse> Configure(Action<HttpClient> func);
        
        Task<HttpResponseMessage> RequestAsync(Func<HttpClient, Task<HttpResponseMessage>> func);
        
        IProxyManager<TClient, TResponse> UseValidator(Func<HttpResponseMessage, Task<bool>> func);
    }
}