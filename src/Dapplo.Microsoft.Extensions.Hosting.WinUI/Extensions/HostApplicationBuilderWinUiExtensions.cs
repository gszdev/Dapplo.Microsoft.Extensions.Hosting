// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Microsoft.Extensions.Hosting.WinUI.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace Dapplo.Microsoft.Extensions.Hosting.WinUI;

/// <summary>
/// This contains the WinUI extensions for Microsoft.Extensions.Hosting
/// </summary>
public static class HostApplicationBuilderWinUiExtensions
{
    #region using a generic extension method for both IHostApplicationBuilder and HostApplicationBuilder will lead to an abmious reference
    // Details: The HostBuilderWinUIExtensions.ConfigureWinUI for IHostBuilder will be used be the compiler
    /*
    /// <summary>
    /// Configure a WinUI application
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <returns></returns>
    public static T ConfigureWinUI<T, TApp, TAppWindow>(this T hostApplicationBuilder)
        where T : IHostApplicationBuilder
        where TApp : Application
        where TAppWindow : Window =>    
        (T)InternalBuilderWinUIUtility.ConfigureWinUI<TApp, TAppWindow>(hostApplicationBuilder);
    */
    #endregion

    /// <summary>
    /// Configure a WinUI application
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureWinUI<TApp, TAppWindow>(this IHostApplicationBuilder hostApplicationBuilder)
        where TApp : Application
        where TAppWindow : Window =>
        InternalBuilderWinUIUtility.ConfigureWinUI<TApp, TAppWindow>(hostApplicationBuilder);

    /// <summary>
    /// Configure a WinUI application
    /// </summary>
    /// <param name="hostApplicationBuilder">HostApplicationBuilder</param>
    /// <returns></returns>
    public static HostApplicationBuilder ConfigureWinUI<TApp, TAppWindow>(this HostApplicationBuilder hostApplicationBuilder)
        where TApp : Application
        where TAppWindow : Window =>
        (HostApplicationBuilder)InternalBuilderWinUIUtility.ConfigureWinUI<TApp, TAppWindow>(hostApplicationBuilder);

}
