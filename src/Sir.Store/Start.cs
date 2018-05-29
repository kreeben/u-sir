using Microsoft.Extensions.DependencyInjection;
using System;

namespace Sir.Store
{
    public class Start : IPluginStart
    {
        public void OnApplicationStartup(IServiceCollection services)
        {
            services.AddSingleton(typeof(SessionFactory), new SessionFactory());
        }
    }

    public class Stop : IPluginStop
    {
        public void OnApplicationShutdown(IServiceProvider serviceProvider)
        {
            var sessionFactory = serviceProvider.GetService<SessionFactory>();
            sessionFactory.Dispose();
        }
    }
}
