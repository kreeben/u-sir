using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sir.HttpServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "App_Plugins");
            var postActions = new WriteActionCollection();

            foreach(var file in Directory.GetFiles(pluginDir, "*.dll"))
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);

                foreach(var pluginType in assembly.GetTypes().Where(t => typeof(IWriteAction).IsAssignableFrom(t)))
                {
                    var pluginInstance = (IWriteAction)Activator.CreateInstance(pluginType);
                    postActions.Add(pluginInstance.ContentType, pluginInstance);
                }
            }

            services.AddSingleton(typeof(WriteActionCollection), postActions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
