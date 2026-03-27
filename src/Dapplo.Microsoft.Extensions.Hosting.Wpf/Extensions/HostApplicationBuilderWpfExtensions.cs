// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.Wpf;

/// <summary>
/// This contains the WPF extensions for Microsoft.Extensions.Hosting
/// </summary>
public static class HostApplicationBuilderWpfExtensions
{
    /// <summary>
    /// Defines that stopping the WPF application also stops the host (application)
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <param name="shutdownMode">ShutdownMode default is OnLastWindowClose</param>
    /// <returns>IHostApplicationBuilder</returns>
    public static T UseWpfLifetime<T>(this T hostApplicationBuilder, ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose)
       where T : IHostApplicationBuilder
       => (T)InternalBuilderWpfUtility.UseWpfLifetime(hostApplicationBuilder, shutdownMode);

    /// <summary>
    /// Configure an WPF application 
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <param name="configureDelegate">Action to configure Wpf</param>
    /// <returns></returns>
    public static T ConfigureWpf<T>(this T hostApplicationBuilder, Action<IWpfBuilder> configureDelegate = null)
        where T : IHostApplicationBuilder
        => (T)InternalBuilderWpfUtility.ConfigureWpf(hostApplicationBuilder, configureDelegate);
}
