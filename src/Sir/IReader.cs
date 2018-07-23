using System.IO;

namespace Sir
{
    public interface IReader : IPlugin
    {
        void Read(IModelFormatter modelBinder, Query query, Stream output);
    }
}
