using System.Collections.Generic;

namespace Sir
{
    public class WriteOperationCollection
    {
        private readonly Dictionary<string, IList<IWriteOperation>> _services;

        public WriteOperationCollection()
        {
            _services = new Dictionary<string, IList<IWriteOperation>>();
        }

        public void Add(string mediaType, IWriteOperation postAction)
        {
            if (!_services.ContainsKey(mediaType))
            {
                _services.Add(mediaType, new List<IWriteOperation>());
            }
            _services[mediaType].Insert(postAction.Ordinal, postAction);
        }

        public IEnumerable<IWriteOperation> GetMany(string mediaType)
        {
            IList<IWriteOperation> services;

            if (_services.TryGetValue(mediaType, out services))
            {
                foreach (var service in services)
                {
                    yield return service;
                }
            }
            
            if (_services.TryGetValue(string.Empty, out services))
            {
                foreach (var service in services)
                {
                    yield return service;
                }
            }
        }
    }
}
