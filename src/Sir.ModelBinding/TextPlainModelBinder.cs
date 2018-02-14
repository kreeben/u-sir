using System;
using System.IO;

namespace Sir.ModelBinding
{
    public class TextPlainModelBinder : IModelBinder
    {
        public string MediaType => "text/plain";

        public void Persist(Stream data)
        {
        }
    }
}
