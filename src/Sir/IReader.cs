using System.IO;

namespace Sir
{
    public interface IReader : IPlugin
    {
        Stream Read(string id, IModelBinder modelBinder, Query query = null);
    }
}
