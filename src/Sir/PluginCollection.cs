using System;
using System.Collections.Generic;
using System.Linq;

namespace Sir
{
    public class PluginCollection : IDisposable
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
            var list = _services[key][t];
            list.Add(service);
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
                        if (p != null)
                            yield return (T)p;
                    }
                }
            }
        }

        ~PluginCollection()
        {
            if (_services.Count > 0)
                Dispose();
        }

        public void Dispose()
        {
            foreach(var s in _services.Values.SelectMany(x => x.Values.SelectMany(y => y)))
            {
                s.Dispose();
            }
            _services.Clear();
        }
    }
}
