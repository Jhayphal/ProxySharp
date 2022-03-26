namespace ProxySharp.Providers.Selectors
{
	/// <summary>
	/// Provides an algorithm for picking proxies from the list.
	/// </summary>
	public interface IProxySelector
	{
		/// <summary>
		/// Fired when the current proxy changes.
		/// </summary>
		event EventHandler<ProxyInfo>? CurrentChanged;

		/// <summary>
		/// Proxy list provider.
		/// </summary>
		IProxyProvider Provider { get; }

		/// <summary>
		/// Current proxy server.
		/// </summary>
		ProxyInfo Current { get; }

		/// <summary>
		/// Sets the current proxy at start.
		/// </summary>
		void SetCurrent();

		/// <summary>
		/// Sets the next proxy.
		/// </summary>
		void SetCurrentToNext();
	}
}
