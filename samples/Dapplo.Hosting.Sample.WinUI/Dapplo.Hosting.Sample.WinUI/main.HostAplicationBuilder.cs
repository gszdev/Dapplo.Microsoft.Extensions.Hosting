#if USE_HOST_APPLICATION_BUILDER

using Dapplo.Microsoft.Extensions.Hosting.WinUI;
using Hosting.Sample.WinUI;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.ConfigureWinUI<App, MainWindow>();

var host = builder.Build();

host.Run();
#endif
