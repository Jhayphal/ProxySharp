using System.Net;
using System.Xml.Serialization;

namespace ProxySharp
{
	/// <summary>
	/// Information about the proxy server.
	/// </summary>
	public class ProxyInfo : IEquatable<ProxyInfo>
	{
		public string Host { get; set; } = string.Empty;

		public int Port { get; set; }

		[XmlElement(IsNullable = false)]
		public string? UserName { get; set; }

		[XmlElement(IsNullable = false)]
		public string? Password { get; set; }

		public int FailsCount { get; set; }

		public int UsedCount { get; set; }

		[XmlIgnore]
		public Exception? LastException { get; set; }

		[XmlIgnore]
		public bool IsEmpty => string.IsNullOrEmpty(Host) || Port == 0;

		[XmlIgnore]
		public float Rating => UsedCount > 0
			? 1f - FailsCount / (float)UsedCount
			: 1f;

		[XmlIgnore]
		public bool HasCredentials => !(string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password));

		public static ProxyInfo Empty { get; } = new ProxyInfo();

		public bool Equals(ProxyInfo? obj)
		{
			if (obj == null)
				return false;

			return string.Equals(Host, obj.Host)
				&& Port == obj.Port
				&& string.Equals(UserName, obj.UserName)
				&& string.Equals(Password, obj.Password);
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj is ProxyInfo info)
				return Equals(info);

			return false;
		}

		public override int GetHashCode()
		{
			return Host.GetHashCode() ^ Port.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Host?.Trim()}:{Port}";
		}

		public virtual WebProxy ToWebProxy()
        {
			var webProxy = new WebProxy(Host, Port);

			if (HasCredentials)
				webProxy.Credentials = new NetworkCredential(UserName, Password);

			return webProxy;
		}
	}
}