Service pages via HTTPS, SSL setup
==================================

The service is designed to be deployed as an Azure Web App, reusing the
SSL encryption provided by the platform, to expose, for example, private
services hosted in Azure VMs, Cloud Apps, etc.

However, you can easily extend the code to use a custom certificate,
editing
[Program.cs](https://github.com/Azure/reverse-proxy-dotnet/blob/master/ProxyAgent/Program.cs)
and using something like this:

```
var host = new WebHostBuilder()
    .UseKestrel(options =>
    {
        options.UseHttps("my-cert.pfx", "my-cert-password");
    })
    .UseUrls("https://*:" + config.Port)
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseIISIntegration()
    .UseStartup<Startup>()
    .Build();
```

See the
[Kestrel documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel)
for more information about this setup and other options.