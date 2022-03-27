﻿namespace ProxySharp.Providers
{
	/// <summary>
	/// Proxy list provider.
	/// </summary>
	public interface IProxyProvider
	{
		/// <summary>
		/// List of proxies.
		/// </summary>
		public IEnumerable<ProxyInfo> Proxies { get; }
	}
}