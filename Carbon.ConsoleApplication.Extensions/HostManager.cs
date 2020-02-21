﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Carbon.ConsoleApplication
{
    public static class HostManager
    {
        public static async Task RunAsync<TProgram>(Action<HostBuilderContext, IConfigurationBuilder> configureApp, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            await new HostBuilder().UseCarbonConfigureApplication<TProgram>(configureApp)
                                   .UseCarbonConfigureServices<TProgram>(configureServices)
                                   .RunConsoleAsync();
        }

        public static async Task RunAsync<TProgram>(Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class
        {
            await new HostBuilder().UseCarbonConfigureApplication<TProgram>(configureApp).RunConsoleAsync();
        }

        public static async Task RunAsync<TProgram>(Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            await new HostBuilder().UseCarbonConfigureServices<TProgram>(configureServices).RunConsoleAsync();
        }

    }
}