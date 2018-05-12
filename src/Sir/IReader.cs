using System.IO;

namespace Sir
{
    public interface IReader : IPlugin
    {
        Stream Read(string id, IModelParser modelBinder, Query query = null);
    }
}
