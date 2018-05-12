using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace Sir.HttpServer
{
    public static class PluginFactory
    {
        public static void Configure(IServiceCollection services)
        {
            var assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "bin\\Release\\netcoreapp2.0");
#if DEBUG
            assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "bin\\Debug\\netcoreapp2.0");
#endif
            foreach (var assembly in Directory.GetFiles(assemblyPath, "*.plugin.dll")
                .Select(file=> AssemblyLoadContext.Default.LoadFromAssemblyPath(file)))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsInterface)
                    {
                        var interfaces = type.GetInterfaces();
                        var firstInterface = interfaces.FirstOrDefault();
                        var contract = firstInterface ?? type;
                        var lastInterface = interfaces.LastOrDefault() ?? contract;

                        if (lastInterface == typeof(IPlugin))
                        {
                            services.Add(new ServiceDescriptor(
                                contract, type, ServiceLifetime.Singleton));
                        }
                        else if (lastInterface == typeof(IPluginConfiguration))
                        {
                            Activator.CreateInstance<IPluginConfiguration>().Configure(services);
                        }
                    }
                }
            }

            services.Add(new ServiceDescriptor(typeof(PluginCollection), new PluginCollection()));

            var serviceProvider = services.BuildServiceProvider();
            var plugins = serviceProvider.GetService<PluginCollection>();

            foreach (var service in serviceProvider.GetServices<IModelParser>())
            {
                plugins.Add(service.ContentType, service);
            }
            foreach (var service in serviceProvider.GetServices<IModelFormatter>())
            {
                plugins.Add(service.ContentType, service);
            }
            foreach (var service in serviceProvider.GetServices<IWriter>())
            {
                plugins.Add(service.ContentType, service);
            }
            foreach (var service in serviceProvider.GetServices<IReader>())
            {
                plugins.Add(service.ContentType, service);
            }
            foreach (var service in serviceProvider.GetServices<IQueryParser>())
            {
                plugins.Add(service.ContentType, service);
            }
        }
    }
}
