namespace ProxySharp.Providers
{
	internal class ProxyList : IProxyProvider
	{
		public IEnumerable<ProxyInfo> Proxies { get; private set; }

		/// <param name="list">Proxy list..</param>
		/// <remarks>Lines like a "1.1.1.1:80".</remarks>
		public ProxyList(string list)
		{
			var result = new HashSet<ProxyInfo>();

			foreach (var line in list.Split())
			{
				try
				{
					var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length != 2)
						continue;

					if (!int.TryParse(parts[1], out int port))
						continue;

					result.Add(new ProxyInfo
					{
						Host = parts[0].Trim(),
						Port = port
					});
				}
				catch (Exception e)
				{
					e.LogError();
				}
			}

			Proxies = result;
		}
	}
}