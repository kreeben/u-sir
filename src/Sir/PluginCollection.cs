using System;
using System.Collections.Generic;
using System.Linq;

namespace Sir
{
    public class PluginCollection
    {
        private readonly IDictionary<string, IDictionary<Type, IList<IPlugin>>> _services;

        public PluginCollection()
        {
            _services = new Dictionary<string, IDictionary<Type, IList<IPlugin>>>();
        }

        public void Add<T>(string key, T service) where T : IPlugin
        {
            if (!_services.ContainsKey(key))
            {
                _services.Add(key, new Dictionary<Type, IList<IPlugin>>());
            }
            var t = typeof(T);
            if(!_services[key].ContainsKey(t))
            {
                _services[key].Add(t, new List<IPlugin>());
            }
            _services[key][t].Insert(service.Ordinal, service);
        }

        public IEnumerable<string> Keys { get { return _services.Keys; } }

        public T Get<T>(string key) where T : IPlugin
        {
            return All<T>(key).FirstOrDefault();
        }

        public IEnumerable<T> All<T>(string key) where T : IPlugin
        {
            foreach (var s in Services<T>(string.Empty))
            {
                yield return s;
            }
            foreach (var s in Services<T>(key))
            {
                yield return s;
            }
        }

        private IEnumerable<T> Services<T>(string key) where T : IPlugin
        {
            IDictionary<Type, IList<IPlugin>> services;

            if (_services.TryGetValue(key, out services))
            {
                IList<IPlugin> plugins;
                if (services.TryGetValue(typeof(T), out plugins))
                {
                    foreach(var p in plugins)
                    {
                        yield return (T)p;
                    }
                }
            }
        }
    }
}
