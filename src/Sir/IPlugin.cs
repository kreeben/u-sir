namespace Sir
{
    public interface IPlugin
    {
        string ContentType { get; }
        int Ordinal { get; }
    }
}
