#if USE_HOST_APPLICATION_BUILDER
// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dapplo.Hosting.Sample.Common;
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
    private const string HostSettingsFile = "hostsettings.json";
    private const string Prefix = "PREFIX_";

    public static Task Main(string[] args)
    {
        var executableLocation = ProgramUtility.GetExecutableDirectoryName() ?? throw new NotSupportedException("Can't start without location.");
        var hostApplicationBuilderSettings = new HostApplicationBuilderSettings()
        {
            Args = args,
        };

        // Issue loading environment name from hostsettings.json file
        // For more details see https://github.com/dotnet/runtime/issues/97930 (Unable to configure host environment from a JSON settings file when using Host.CreateApplicationBuilder)
        var environmentName = Dapplo.Hosting.Sample.Common.HostingUtility.GetEnvironmentNameFromHostSettingsFile(HostSettingsFile);
        if (!string.IsNullOrEmpty(environmentName))
        {
            hostApplicationBuilderSettings.EnvironmentName = environmentName;
        }

        var builder = Host.CreateApplicationBuilder(hostApplicationBuilderSettings);
        builder.Configuration.AddEnvironmentVariables(prefix: Prefix);

        builder.ConfigureSplatForMicrosoftDependencyResolver();

        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        builder.ConfigureSingleInstance(builder =>
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
        .ConfigureWpf(wpfBuilder =>
        {
            wpfBuilder.UseWindow<MainWindow>();

            var rxAppBuilder = ReactiveUI.Builder.RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithViewsFromAssembly(Assembly.GetExecutingAssembly())
            .WithRegistration(locator => {
                //locator.InitializeSplat();
            });

            var rxApp = rxAppBuilder.BuildApp();
            //rxApp.MainThreadScheduler
            //rxApp.TaskpoolScheduler
            builder.Services.AddSingleton(rxApp);
            
        })
        //.UseConsoleLifetime()
        .UseWpfLifetime();

        builder.Services.UseMicrosoftDependencyResolver();

        // See https://reactiveui.net/docs/handbook/routing to learn more about routing in RxUI
        builder.Services.AddTransient<IViewFor<NugetDetailsViewModel>, NugetDetailsView>();
        builder.Services.AddTransient<AppViewModel>();

        var host = builder.Build();
        Console.WriteLine("Run!");
        return host.RunAsync();
    }
}

#endif
