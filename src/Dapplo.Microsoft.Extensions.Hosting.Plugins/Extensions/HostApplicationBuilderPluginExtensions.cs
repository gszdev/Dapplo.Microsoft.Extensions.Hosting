// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapplo.Microsoft.Extensions.Hosting.Plugins.Extensions;

#if NETCOREAPP
using System.Runtime.Loader;
#endif
using Dapplo.Microsoft.Extensions.Hosting.Plugins.Internals;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins;

/// <summary>
/// Extensions for adding plug-ins to your host
/// </summary>
public static class HostApplicationBuilderPluginExtensions
{    
    /// <summary>
    /// Configure the plugins
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <param name="configurePlugin">Action to configure the IPluginBuilder</param>
    /// <returns>IHostApplicationBuilder</returns>
    public static T ConfigurePlugins<T>(this T hostApplicationBuilder, Action<IPluginBuilder> configurePlugin)
        where T : IHostApplicationBuilder =>    
        (T) InternalBuilderPluginUtility.ConfigurePlugins(hostApplicationBuilder, configurePlugin);
    
    /*
    /// <summary>
    /// Configure the plugins
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <param name="configurePlugin">Action to configure the IPluginBuilder</param>
    /// <returns>IHostApplicationBuilder</returns>
    public static IHostApplicationBuilder ConfigurePlugins(this IHostApplicationBuilder hostApplicationBuilder, Action<IPluginBuilder> configurePlugin) =>
        InternalBuilderPluginUtility.ConfigurePlugins(hostApplicationBuilder, configurePlugin);

    /// <summary>
    /// Configure the plugins
    /// </summary>
    /// <param name="hostApplicationBuilder">HostApplicationBuilder</param>
    /// <param name="configurePlugin">Action to configure the IPluginBuilder</param>
    /// <returns>HostApplicationBuilder</returns>
    public static HostApplicationBuilder ConfigurePlugins(this HostApplicationBuilder hostApplicationBuilder, Action<IPluginBuilder> configurePlugin) =>
        (HostApplicationBuilder)InternalBuilderPluginUtility.ConfigurePlugins(hostApplicationBuilder, configurePlugin);
    */
}
