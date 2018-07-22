using System.IO;

namespace Sir
{
    public interface IReader : IPlugin
    {
        Stream Read(IModelFormatter modelBinder, Query query);
    }
}
