using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxySharp.Providers.Selectors
{
	public sealed class OnlyValidProxySelector : ProxySelectorBase
	{
		public OnlyValidProxySelector(IProxyProvider provider)
			: base(provider) { }

		public override void SetCurrentToNext()
		{
			var enumerator = GetActual()
				.GetEnumerator();

			if (!enumerator.MoveNext())
			{
				enumerator.Reset();

				if (!enumerator.MoveNext())
					throw new InvalidOperationException("Has no proxies.");
			}

			Current = enumerator.Current;
		}

		protected override IEnumerable<ProxyInfo> GetActual()
		{
			return Provider.Proxies
				.Where(proxy => proxy != null && proxy.LastException == null);
		}
	}
}
