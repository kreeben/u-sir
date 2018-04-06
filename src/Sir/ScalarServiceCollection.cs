using System.Collections.Generic;

namespace Sir
{
    public abstract class ScalarServiceCollection<T>
    {
        protected Dictionary<string, T> Services { get; set; }

        public virtual void Add(string mediaType, T service)
        {
            Services[mediaType] = service;
        }

        public ScalarServiceCollection()
        {
            Services = new Dictionary<string, T>();
        }

        public virtual T Get(string mediaType)
        {
            T service;
            if (Services.TryGetValue(mediaType, out service))
            {
                return service;
            }
            return default(T);
        }
    }
}
