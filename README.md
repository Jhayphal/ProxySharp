# ProxySharp
Provides functionality for executing requests via proxy servers. The list of proxy servers and the selection algorithm is easily expanded by implementing the necessary interfaces. If the proxy server does not respond or the response does not pass user validation, the request is executed through the next available proxy server.

## Usage expamle

```
// Load if needed
// var proxies = ProxyStorage.Load("proxies.xml");
// var proxyList = new ProxyList(proxies);
			
// Or
var proxyList = new ProxyList {
  new ProxyInfo
  {
    Host = "1.1.1.1",
    Port = 8080
  },
  new ProxyInfo
  {
    Host = "2.2.2.2",
    Port = 8081,
    UserName = "user",
    Password = "password"
  },
};

// Save if needed
// ProxyStorage.Save("proxies.xml", proxyList);

var selector = new PrioritizeBestRatingProxySelector(proxyList);

var manager = new ProxyManager(selector);

manager
  .Configure(client => 
    client.Timeout = TimeSpan.FromSeconds(60))

  .UseValidator(async response => 
    (await response.Content.ReadAsStringAsync())
      .Contains("marker of successful"));

var result = await manager.RequestAsync(async client => await client.GetAsync("google.com"));
var pageContent = await result.Content.ReadAsStringAsync();

Console.WriteLine(pageContent);
```

For manual change proxy between requests use:

```
manager.ChangeProxy();
```

## Details

You can implement a custom proxy provider by implementing the interface `IProxyProvider`. For example, your provider can parse a website with a free proxy list.

Every `ProxyInfo` has `Rating` and `LastException` properties. You can change the algorithm that chooses the first and next proxies by implementing the interface `IProxySelector`.
