using System.IO;

namespace Sir
{
    public interface IWriteAction
    {
        int Ordinal { get; }
        string ContentType { get; } 
        void Execute(string table, Stream data);
    }
}
