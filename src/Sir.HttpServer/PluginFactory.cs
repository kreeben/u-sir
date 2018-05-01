using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            foreach (var assembly in Directory.GetFiles(assemblyPath, "*.dll")
                .Select(file=> AssemblyLoadContext.Default.LoadFromAssemblyPath(file)))
            {
                foreach (var service in LoadPlugins<IModelBinder>(assembly))
                {
                    plugins.Add(service.ContentType, service);
                }
                foreach (var service in LoadPlugins<IWriter>(assembly))
                {
                    plugins.Add(service.ContentType, service);
                }
                foreach (var service in LoadPlugins<IQueryParser>(assembly))
                {
                    plugins.Add(service.ContentType, service);
                }
            }
            services.AddSingleton(typeof(PluginCollection), plugins);
        }

        public static IEnumerable<T> LoadPlugins<T>(Assembly assembly) 
            where T : IPlugin
        {
            var type = typeof(T);

            foreach (var pluginType in assembly.GetTypes()
                    .Where(t => type.IsAssignableFrom(t)))
            {
                if (!pluginType.IsInterface)
                {
                    var pluginInstance = (T)Activator.CreateInstance(pluginType);
                    yield return pluginInstance;
                }
            }
        }
    }
}
