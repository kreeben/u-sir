using Microsoft.Extensions.DependencyInjection;
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
            var plugins = new PluginCollection();

            foreach (var assembly in Directory.GetFiles(assemblyPath, "*.plugin.dll")
                .Select(file=> AssemblyLoadContext.Default.LoadFromAssemblyPath(file)))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsInterface)
                    {
                        var firstInterface = type.GetInterfaces().FirstOrDefault();
                        var contract = firstInterface ?? type;

                        if (contract == typeof(IModelBinder) ||
                            contract == typeof(IWriter) ||
                            contract == typeof(IReader) ||
                            contract == typeof(IQueryParser))
                        {
                            services.Add(new ServiceDescriptor(
                                contract, type, ServiceLifetime.Singleton));
                        }
                        else
                        {
                            services.Add(new ServiceDescriptor(
                                contract, type, ServiceLifetime.Transient));
                        }
                    }
                }
            }
            services.Add(new ServiceDescriptor(typeof(PluginCollection), plugins));

            var serviceProvider = services.BuildServiceProvider();

            foreach (var service in serviceProvider.GetServices<IModelBinder>())
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
