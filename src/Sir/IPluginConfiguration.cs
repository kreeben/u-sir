using Microsoft.Extensions.DependencyInjection;

namespace Sir
{
    public interface IPluginConfiguration
    {
        void Configure(IServiceCollection services);
    }
}
