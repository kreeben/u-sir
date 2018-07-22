namespace Sir
{
    public interface IQueryParser : IPlugin
    {
        Query Parse(ulong collectionId, string query);
    }
}
