using System.IO;

namespace Sir.Store
{
    public class StreamProvider
    {
        public Stream ValueStream { get; set; }
        public Stream KeyStream { get; set; }
        public Stream KeyDictionaryStream { get; set; }
        public Stream IndexStream { get; set; }
    }
}
