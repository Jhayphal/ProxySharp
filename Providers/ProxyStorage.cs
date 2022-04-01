using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProxySharp.Providers
{
	public sealed class ProxyStorage : IProxyProvider
	{
		public readonly string FileName;

		public IEnumerable<ProxyInfo> Proxies { get; private set; }

		public ProxyStorage(string fileName)
		{
			FileName = fileName;
			Proxies = Load(fileName);
		}

		public bool Save()
		{
			return Save(FileName, Proxies);
		}

		public static IEnumerable<ProxyInfo> Load(string fileName)
		{
			if (!File.Exists(fileName))
				return Enumerable.Empty<ProxyInfo>();

			var serializer = new XmlSerializer(typeof(ProxyInfo[]));

			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				return (ProxyInfo[])serializer.Deserialize(stream) ?? Enumerable.Empty<ProxyInfo>();
		}

		public static bool Save(string fileName, IProxyProvider provider)
		{
			return Save(fileName, provider.Proxies);
		}

		public static bool Save(string fileName, IEnumerable<ProxyInfo> proxies)
		{
			try
			{
				var data = proxies.ToArray();

				var serializer = new XmlSerializer(data.GetType());

				using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
					serializer.Serialize(stream, data);

				return true;
			}
			catch (Exception error)
			{
				error.LogError();

				return false;
			}
		}
	}
}
