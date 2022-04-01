using System.Collections.Generic;

namespace ProxySharp.Providers
{
	/// <summary>
	/// Proxy list provider.
	/// </summary>
	public interface IProxyProvider
	{
		/// <summary>
		/// List of proxies.
		/// </summary>
		IEnumerable<ProxyInfo> Proxies { get; }
	}
}