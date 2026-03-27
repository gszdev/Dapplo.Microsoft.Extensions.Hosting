#if !USE_HOST_APPLICATION_BUILDER
// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Plugins;
using Dapplo.Microsoft.Extensions.Hosting.ReactiveUI;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Builder;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Dapplo.Hosting.Sample.ReactiveDemo;

public static class Program
{
    private const string AppSettingsFilePrefix = "appsettings";
    private const string HostSettingsFile = "hostsettings.json";
    private const string Prefix = "PREFIX_";

    public static Task Main(string[] args)
    {
        //var app1 = ReactiveUI.Builder.RxAppBuilder.CreateReactiveUIBuilder();


        var executableLocation = ProgramUtility.GetExecutableDirectoryName() ?? throw new NotSupportedException("Can't start without location.");
        var host = new HostBuilder()
            .ConfigureSplatForMicrosoftDependencyResolver()

            .ConfigureLogging()
            .ConfigureConfiguration(args)
            .ConfigureSingleInstance(builder =>
            {
                builder.MutexId = "{EDF77D19-3272-43FF-81E1-AB36D08397EE}";
                builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
                {
                    // This is called when an instance was already started, this is in the second instance
                    logger.LogWarning("Application {ApplicationName} already running.", hostingEnvironment.ApplicationName);
                };
            })
            .ConfigurePlugins(pluginBuilder =>
            {
                var runtime = Path.GetFileName(executableLocation);
                var parentDirectory = Directory.GetParent(executableLocation).FullName;
                var configuration = Path.GetFileName(parentDirectory);
                var basePath = Path.Combine(executableLocation, @"..\..\..\..\");
                // Specify the location from where the Dll's are "globbed"
                pluginBuilder.AddScanDirectories(basePath);
                // Add the framework libraries which can be found with the specified globs
                pluginBuilder.IncludeFrameworks(@$"**\bin\{configuration}\netstandard2.0\*.FrameworkLib.dll");
                // Add the plugins which can be found with the specified globs
                pluginBuilder.IncludePlugins(@$"**\bin\{configuration}\{runtime}\*.Sample.Plugin*.dll");
            })
            .ConfigureServices(serviceCollection =>
            {
                // Make sure we got all the ReactiveUI setup
                serviceCollection.UseMicrosoftDependencyResolver();
                /*
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();
                */

                var rxAppBuilder = ReactiveUI.Builder.RxAppBuilder.CreateReactiveUIBuilder()
                .WithWpf()
                .WithViewsFromAssembly(Assembly.GetExecutingAssembly())
                .WithRegistration(locator =>
                {

                });

                var rxApp = rxAppBuilder.BuildApp();
                serviceCollection.AddSingleton(rxApp);

                // See https://reactiveui.net/docs/handbook/routing to learn more about routing in RxUI
                serviceCollection.AddTransient<IViewFor<NugetDetailsViewModel>, NugetDetailsView>();
                serviceCollection.AddTransient<AppViewModel>();
            })
            .ConfigureWpf(wpfBuilder => {
                wpfBuilder.UseWindow<MainWindow>();

                
            })
            .UseConsoleLifetime()
            .UseWpfLifetime()
            .Build();

        Console.WriteLine("Run!");
        return host.RunAsync();
    }

    /// <summary>
    /// Configure the loggers
    /// </summary>
    /// <param name="hostBuilder">IHostBuilder</param>
    /// <returns>IHostBuilder</returns>
    private static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureLogging((hostContext, configLogging) =>
            configLogging
                .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug());

    /// <summary>
    /// Configure the configuration
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private static IHostBuilder ConfigureConfiguration(this IHostBuilder hostBuilder, string[] args) =>
        hostBuilder.ConfigureHostConfiguration(configHost =>
            configHost.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(HostSettingsFile, optional: true)
                .AddEnvironmentVariables(prefix: Prefix)
                .AddCommandLine(args))
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.AddJsonFile(AppSettingsFilePrefix + ".json", optional: true);
                if (!string.IsNullOrEmpty(hostContext.HostingEnvironment.EnvironmentName))
                {
                    configApp.AddJsonFile(AppSettingsFilePrefix + $".{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                }
                configApp.AddEnvironmentVariables(prefix: Prefix).AddCommandLine(args);
            });
}
#endif
