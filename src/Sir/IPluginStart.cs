using Microsoft.Extensions.DependencyInjection;

namespace Sir
{
    public interface IPluginStart
    {
        void OnApplicationStartup(IServiceCollection services);
    }
}
