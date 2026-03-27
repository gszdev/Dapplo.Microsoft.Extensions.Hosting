using System;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Dapplo.Microsoft.Extensions.Hosting.Plugins;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Dapplo.Hosting.Sample.DefaultWpfDemo;

public static class AppMixins
{    
    internal const string HostSettingsFile = "hostsettings.json";
#if !USE_HOST_APPLICATION_BUILDER
    private const string AppSettingsFilePrefix = "appsettings";
#endif
    internal const string Prefix = "PREFIX_";
}
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
#if USE_HOST_APPLICATION_BUILDER
    public App()
    {
        var executableLocation = Path.GetDirectoryName(typeof(App).Assembly.Location) ?? throw new NotSupportedException("Can't start without location.");

        var hostApplicationBuilderSettings = new HostApplicationBuilderSettings() { Args = Environment.GetCommandLineArgs(), };
        
        // Issue loading environment name from hostsettings.json file
        // For more details see https://github.com/dotnet/runtime/issues/97930 (Unable to configure host environment from a JSON settings file when using Host.CreateApplicationBuilder)
        var environmentName = Dapplo.Hosting.Sample.Common.HostingUtility.GetEnvironmentNameFromHostSettingsFile(AppMixins.HostSettingsFile);
        if (!string.IsNullOrEmpty(environmentName))
        {
            hostApplicationBuilderSettings.EnvironmentName = environmentName;
        }

        var builder = Host.CreateApplicationBuilder(hostApplicationBuilderSettings);
        builder.Configuration.AddEnvironmentVariables(prefix: AppMixins.Prefix);

        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        builder
           .ConfigureSingleInstance(builder =>
           {
               builder.MutexId = "{326b35ad-e777-45b9-8fe4-f5edebf9bb2d}";
               builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
                   // This is called when an instance was already started, this is in the second instance
                   logger.LogWarning("Application {ApplicationName} already running.", hostingEnvironment.ApplicationName);
           })
           .ConfigurePlugins(pluginBuilder =>
           {
               var runtime = Path.GetFileName(executableLocation);
               var parentDirectory = Directory.GetParent(executableLocation)?.FullName;
               var configuration = Path.GetFileName(parentDirectory);
               var basePath = Path.Combine(executableLocation, @"..\..\..\..\");
               // Specify the location from where the Dll's are "globbed"
               pluginBuilder.AddScanDirectories(basePath);
               // Add the framework libraries which can be found with the specified globs
               pluginBuilder.IncludeFrameworks(@$"**\bin\{configuration}\netstandard2.0\*.FrameworkLib.dll");
               // Add the plugins which can be found with the specified globs
               pluginBuilder.IncludePlugins(@$"**\bin\{configuration}\{runtime}\*.Sample.Plugin*.dll");
           })           
           .ConfigureWpf(wpfBuilder => wpfBuilder.UseCurrentApplication(this).UseWindow<MainWindow>())
           .UseWpfLifetime();

        // Make OtherWindow available for DI to the MainWindow, but not as singleton
        builder.Services.AddTransient<OtherWindow>();

        var host = builder.Build();

        Console.WriteLine("Run!");

        host.RunAsync();
    }
#else
    public App()
    {
        var executableLocation = Path.GetDirectoryName(typeof(App).Assembly.Location) ?? throw new NotSupportedException("Can't start without location.");
        var host = new HostBuilder()
            .ConfigureLogging()
            .ConfigureConfiguration(Environment.GetCommandLineArgs())
            .ConfigureSingleInstance(builder =>
            {
                builder.MutexId = "{326b35ad-e777-45b9-8fe4-f5edebf9bb2d}";
                builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
                    // This is called when an instance was already started, this is in the second instance
                    logger.LogWarning("Application {ApplicationName} already running.", hostingEnvironment.ApplicationName);
            })
            .ConfigurePlugins(pluginBuilder =>
            {
                var runtime = Path.GetFileName(executableLocation);
                var parentDirectory = Directory.GetParent(executableLocation)?.FullName;
                var configuration = Path.GetFileName(parentDirectory);
                var basePath = Path.Combine(executableLocation, @"..\..\..\..\");
                // Specify the location from where the Dll's are "globbed"
                pluginBuilder.AddScanDirectories(basePath);
                // Add the framework libraries which can be found with the specified globs
                pluginBuilder.IncludeFrameworks(@$"**\bin\{configuration}\netstandard2.0\*.FrameworkLib.dll");
                // Add the plugins which can be found with the specified globs
                pluginBuilder.IncludePlugins(@$"**\bin\{configuration}\{runtime}\*.Sample.Plugin*.dll");
            })
            // Make OtherWindow available for DI to the MainWindow, but not as singleton
            .ConfigureServices(serviceCollection => serviceCollection.AddTransient<OtherWindow>())
            .ConfigureWpf(wpfBuilder => wpfBuilder.UseCurrentApplication(this).UseWindow<MainWindow>())
            .UseWpfLifetime()
            .UseConsoleLifetime()
            .Build();

        Console.WriteLine("Run!");

        host.RunAsync();
    }
#endif

}
