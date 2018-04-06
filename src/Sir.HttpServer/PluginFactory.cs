using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace Sir.HttpServer
{
    public static class PluginFactory
    {
        public static ModelBinderCollection LoadModelBinders()
        {
            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "App_Plugins");
            return LoadModelBinders(pluginDir);
        }

        public static ModelBinderCollection LoadModelBinders(string pluginDir)
        {
            return LoadInstances<ModelBinderCollection, IModelBinder>(pluginDir);
        }

        public static WriteOperationCollection LoadWriteOperations()
        {
            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "App_Plugins");
            return LoadWriteOperations(pluginDir);
        }

        public static WriteOperationCollection LoadWriteOperations(string pluginDir)
        {
            var services = new WriteOperationCollection();
            foreach (var file in Directory.GetFiles(pluginDir, "*.dll"))
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                foreach (var pluginType in assembly.GetTypes()
                    .Where(t => typeof(IWriteOperation).IsAssignableFrom(t)))
                {
                    var pluginInstance = (IWriteOperation)Activator.CreateInstance(pluginType);
                    services.Add(pluginInstance.ContentType, pluginInstance);
                }
            }
            return services;
        }

        public static TServiceCollection LoadInstances<TServiceCollection, TTService>(string pluginDir) 
            where TServiceCollection : ScalarServiceCollection<TTService>, new()
            where TTService : IHasContentType
        {
            var services = new TServiceCollection();
            foreach (var file in Directory.GetFiles(pluginDir, "*.dll"))
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                foreach (var pluginType in assembly.GetTypes()
                    .Where(t => typeof(TTService).IsAssignableFrom(t)))
                {
                    var pluginInstance = (TTService)Activator.CreateInstance(pluginType);
                    services.Add(pluginInstance.ContentType, pluginInstance);
                }
            }
            return services;
        }
    }
}
