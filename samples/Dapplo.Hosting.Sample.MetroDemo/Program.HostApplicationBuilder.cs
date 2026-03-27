#if USE_HOST_APPLICATION_BUILDER
// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Dapplo.Hosting.Sample.Common;
using Dapplo.Microsoft.Extensions.Hosting.AppServices;
using Dapplo.Microsoft.Extensions.Hosting.Metro;
using Dapplo.Microsoft.Extensions.Hosting.Plugins;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dapplo.Hosting.Sample.MetroDemo;

public static class Program
{
    private const string HostSettingsFile = "hostsettings.json";
    private const string Prefix = "PREFIX_";

    public static Task Main(string[] args)
    {
        var executableLocation = ProgramUtility.GetExecutableDirectoryName();
        if (executableLocation == null)
        {
            throw new NotSupportedException("Can't start without location.");
        }

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
                builder.MutexId = "{F91C7BE5-549F-409B-AFCC-0833375F045F}";
                builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
                {
                    // This is called when an instance was already started, this is in the second instance
                    logger.LogWarning("Application {@ApplicationName} already running.", hostingEnvironment.ApplicationName);
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
            })
            .ConfigureMetro("Light.Orange")
            .UseWpfLifetime();

        builder.Services.AddTransient<OtherWindow>();

        var host = builder.Build();

        Console.WriteLine("Run!");

        return host.RunAsync();
    }
}
#endif
