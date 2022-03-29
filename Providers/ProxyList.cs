using System.Collections;

namespace ProxySharp.Providers
{
	/// <summary>
	/// A list of proxies.
	/// </summary>
	public class ProxyList : IProxyProvider, IEnumerable<ProxyInfo>
	{
		protected readonly HashSet<ProxyInfo> _proxies = new();

		public IEnumerable<ProxyInfo> Proxies => _proxies;

		public ProxyList() { }

		public ProxyList(IEnumerable<ProxyInfo> proxies)
        {
			Add(proxies);
        }

		/// <summary>
		/// Clear proxies list.
		/// </summary>
		public void Clear()
        {
			_proxies.Clear();
        }

		/// <summary>
		/// Add proxies.
		/// </summary>
		/// <param name="proxies">Proxies.</param>
		public void Add(ProxyInfo proxy)
		{
			_proxies.Add(proxy);
		}

		/// <summary>
		/// Add proxies.
		/// </summary>
		/// <param name="proxies">Proxies.</param>
		public void Add(IEnumerable<ProxyInfo> proxies)
		{
			foreach (var proxy in proxies)
				_proxies.Add(proxy);
		}

		/// <summary>
		/// Add proxies from text.
		/// </summary>
		/// <param name="list">Proxy list.</param>
		/// <remarks>Lines like a "1.1.1.1:80". Only Host and Port supported.</remarks>
		public void Add(string list)
		{
			Add(Parse(list));
		}

		/// <summary>
		/// Parse proxies from text.
		/// </summary>
		/// <param name="list">Proxy list.</param>
		/// <remarks>Lines like a "1.1.1.1:80". Only Host and Port supported.</remarks>
		/// <returns>List of proxies.</returns>
		protected virtual IEnumerable<ProxyInfo> Parse(string list)
        {
			string[] parts;
			int port;

			foreach (var line in list.Split())
			{
				try
				{
					parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length != 2)
						continue;

					if (!int.TryParse(parts[1], out port))
						continue;
				}
				catch (Exception e)
				{
					e.LogError();

					continue;
				}

				yield return new ProxyInfo
				{
					Host = parts[0].Trim(),
					Port = port
				};
			}
		}

        public IEnumerator<ProxyInfo> GetEnumerator()
        {
            foreach(var proxy in _proxies)
				yield return proxy;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}