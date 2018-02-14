using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sir
{
    public interface IModelBinder
    {
        string MediaType { get; }
        void Persist(Stream data);
    }
}
