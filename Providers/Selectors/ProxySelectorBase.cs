namespace ProxySharp.Providers.Selectors
{
	public abstract class ProxySelectorBase : IProxySelector
	{
		protected ProxyInfo _current = ProxyInfo.Empty;

		public event EventHandler<ProxyInfo>? CurrentChanged;

		public ProxyInfo Current
		{
			get => _current;
			protected set
			{
				if (!ReferenceEquals(_current, value))
				{
					_current = value;

					CurrentChanged?.Invoke(this, _current);
				}
			}
		}

		public IProxyProvider Provider { get; }

		public ProxySelectorBase(IProxyProvider provider)
		{
			Provider = provider
				?? throw new ArgumentNullException(nameof(provider));
		}

		public virtual void SetCurrent()
		{
			if (Current == null)
			{
				var enumerator = GetActual().GetEnumerator();

				if (!enumerator.MoveNext())
				{
					enumerator.Reset();

					if (!enumerator.MoveNext())
						throw new InvalidOperationException("Has no proxies.");
				}

				Current = enumerator.Current;
			}
		}

		public virtual void SetCurrentToNext()
		{
			var query = GetActual();

			if (query.Count() == 1)
			{
				Current = query.First();

				return;
			}

			var enumerator = query.GetEnumerator();

			do
			{
				if (!enumerator.MoveNext())
				{
					enumerator.Reset();

					if (!enumerator.MoveNext())
						throw new InvalidOperationException("Has no proxies.");
				}
			}
			while (ReferenceEquals(_current, enumerator.Current));

			Current = enumerator.Current;
		}

		protected abstract IEnumerable<ProxyInfo> GetActual();
	}
}
