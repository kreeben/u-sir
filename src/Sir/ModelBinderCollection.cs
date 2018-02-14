using System;
using System.Collections.Generic;
using System.Text;

namespace Sir
{
    public class ModelBinderCollection
    {
        private Dictionary<string, IModelBinder> _modelBinders;

        public ModelBinderCollection()
        {
            _modelBinders = new Dictionary<string, IModelBinder>();
        }

        public void Add(string mediaType, IModelBinder modelBinder)
        {
            _modelBinders[mediaType] = modelBinder;
        }

        public IModelBinder Get(string mediaType)
        {
            IModelBinder modelBinder;
            if(_modelBinders.TryGetValue(mediaType, out modelBinder))
            {
                return modelBinder;
            }
            return null;
        }
    }
}
