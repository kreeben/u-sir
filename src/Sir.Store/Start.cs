using Microsoft.Extensions.DependencyInjection;

namespace Sir.Store
{
    public class Start : IPluginStart
    {
        public void OnApplicationStartup(IServiceCollection services)
        {
            services.AddSingleton(typeof(SessionFactory), new SessionFactory());
        }
    }
}
