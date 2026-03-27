#if USE_HOST_APPLICATION_BUILDER
// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Microsoft.Extensions.Hosting.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro;
using Dapplo.Hosting.Sample.CaliburnMicroDemo.ViewModels;
using System.Threading.Tasks;

namespace Dapplo.Hosting.Sample.CaliburnMicroDemo;

public static class Program
{
    private const string AppSettingsFilePrefix = "appsettings";
    private const string HostSettingsFile = "hostsettings.json";
    private const string Prefix = "PREFIX_";

    public static Task Main(string[] args)
    {
        var executableLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);

        var hostApplicationBuilderSettings = new HostApplicationBuilderSettings() { Args = args, };

        // Issue loading environment name from hostsettings.json file
        // For more details see https://github.com/dotnet/runtime/issues/97930 (Unable to configure host environment from a JSON settings file when using Host.CreateApplicationBuilder)
        var environmentName = Dapplo.Hosting.Sample.Common.HostingUtility.GetEnvironmentNameFromHostSettingsFile(HostSettingsFile);
        if (!string.IsNullOrEmpty(environmentName))
        {
            hostApplicationBuilderSettings.EnvironmentName = environmentName;
        }

        var builder = Host.CreateApplicationBuilder(hostApplicationBuilderSettings);
        builder.Configuration.AddEnvironmentVariables(prefix: Prefix);

        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        builder
            .ConfigureSingleInstance(builder =>
            {
                builder.MutexId = "{B9CE32C0-59AE-4AF0-BE39-5329AAFF4BE8}";
                builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
                {
                    // This is called when an instance was already started, this is in the second instance
                    logger.LogWarning("Application {applicationName} already running.", hostingEnvironment.ApplicationName);
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
                pluginBuilder.IncludeFrameworks(@$"**\bin\{configuration}\\netstandard2.0\*.FrameworkLib.dll");
                // Add the plugins which can be found with the specified globs
                pluginBuilder.IncludePlugins(@$"**\bin\{configuration}\{runtime}\*.Sample.Plugin*.dll");
            })
            .ConfigureWpf()
            .ConfigureCaliburnMicro<MainViewModel>()
            .UseWpfLifetime();

        builder.Services.AddTransient<OtherViewModel>();

        var host = builder.Build();

        Console.WriteLine("Run!");
        return host.RunAsync();
    }
}
#endif
