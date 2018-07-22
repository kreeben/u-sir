using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Sir.Store
{
    public class Start : IPluginStart
    {
        public void OnApplicationStartup(IServiceCollection services)
        {
            services.AddSingleton(typeof(SessionFactory), new SessionFactory(Path.Combine(Directory.GetCurrentDirectory(), "App_Data")));
            services.AddSingleton(typeof(ITokenizer), new Tokenizer());

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
