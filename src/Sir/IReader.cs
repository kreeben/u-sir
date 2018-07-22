using System.IO;

namespace Sir
{
    public interface IReader : IPlugin
    {
        Stream Read(IModelBinder modelBinder, Query query);
    }
}
