﻿using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.MassTransit.UnitTests.StaticWrappers.IServiceCollectionConfiguratorExtensionsWrapper
{
    public class ServiceCollectionConfiguratorExtensionsWrapper : IServiceCollectionConfiguratorExtensionsWrapper
    {
        public void AddMassTransitBus(IServiceCollection services, Action<IServiceCollectionConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddMassTransitBus(services, configurator);
        }

        public void AddRabbitMqBus(IServiceCollectionConfigurator serviceCollection, IConfiguration configuration, Action<IServiceProvider, IRabbitMqBusFactoryConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddRabbitMqBus(serviceCollection, configuration, configurator);
        }

        public void AddServiceBus(IServiceCollectionConfigurator serviceCollection, IConfiguration configuration, Action<IServiceProvider, IServiceBusBusFactoryConfigurator> configurator)
        {
            IServiceCollectionConfiguratorExtensions.AddServiceBus(serviceCollection, configuration, configurator);
        }
    }
}
