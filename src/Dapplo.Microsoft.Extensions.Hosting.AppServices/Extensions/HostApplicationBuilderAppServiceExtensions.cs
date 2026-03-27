// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dapplo.Microsoft.Extensions.Hosting.AppServices.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dapplo.Microsoft.Extensions.Hosting.AppServices;

/// <summary>
/// Extensions for loading plugins
/// </summary>
public static class HostApplicationBuilderAppServiceExtensions
{
    /// <summary>
    /// Prevent that an application runs multiple times
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <param name="configureAction">Action to configure IMutexBuilder</param>
    /// <returns>IHostApplicationBuilder for fluently calling</returns>
    public static T ConfigureSingleInstance<T>(this T hostApplicationBuilder, Action<IMutexBuilder> configureAction)
        where T : IHostApplicationBuilder =>
        (T)InternalBuilderAppServiceUtility.ConfigureSingleInstance(hostApplicationBuilder, configureAction);
    

    /// <summary>
    /// Prevent that an application runs multiple times
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder</param>
    /// <param name="mutexId">string</param>
    /// <returns>IHostApplicationBuilder for fluently calling</returns>
    public static T ConfigureSingleInstance<T>(this T hostApplicationBuilder, string mutexId)
        where T : IHostApplicationBuilder =>
        (T)InternalBuilderAppServiceUtility.ConfigureSingleInstance(hostApplicationBuilder, builder => builder.MutexId = mutexId);

}
