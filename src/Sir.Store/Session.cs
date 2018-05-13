using System.IO;

namespace Sir.Store
{
    public class Session
    {
        public Stream ValueStream { get; set; }
        public Stream KeyStream { get; set; }
        public Stream DocStream { get; set; }
        public Stream ValueIndexStream { get; set; }
        public Stream KeyIndexStream { get; set; }
        public Stream DocIndexStream { get; set; }
        public Stream PostingsStream { get; set; }
    }
}
