namespace ProxySharp.Providers.Selectors
{
    public sealed class PrioritizeBestRatingProxySelector : ProxySelectorBase
    {
        public PrioritizeBestRatingProxySelector(IProxyProvider provider)
            : base(provider) { }

        protected override IEnumerable<ProxyInfo> GetActual()
        {
            return Provider.Proxies
                .Where(proxy => proxy != null && proxy.LastException == null)
                .OrderByDescending(proxy => proxy.Rating)
                .ToArray();
        }
    }
}
