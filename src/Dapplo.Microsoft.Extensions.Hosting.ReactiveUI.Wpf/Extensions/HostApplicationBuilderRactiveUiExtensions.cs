using System;
using Dapplo.Microsoft.Extensions.Hosting.ReactiveUI.Wpf;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Dapplo.Microsoft.Extensions.Hosting.ReactiveUI;

/// <summary>
/// This contains the ReactiveUi extensions for Microsoft.Extensions.Hosting.
/// </summary>
public static class HostApplicationBuilderRactiveUiExtensions
{
    /// <summary>
    /// Configure a ReactiveUI application.
    /// </summary>
    /// <param name="hostApplicationBuilder">IHostApplicationBuilder.</param>
    /// <returns>IHostApplicationBuilder</returns>
    public static T ConfigureSplatForMicrosoftDependencyResolver<T>(this T hostApplicationBuilder)
        where T : IHostApplicationBuilder =>    
        (T)InternalBuilderReactiveUiUtility.ConfigureSplatForMicrosoftDependencyResolver(hostApplicationBuilder);
}
