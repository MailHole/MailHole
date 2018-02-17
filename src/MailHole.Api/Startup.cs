using System.IO;
using MailHole.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using StackExchange.Redis;

namespace MailHole.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.TryAddSingleton<IConnectionMultiplexer>(provier =>
                ConnectionMultiplexer.Connect(Configuration.GetRedisConnectionString())
            );
            services.TryAddTransient<IDatabaseAsync>(provider =>
                provider.GetService<IConnectionMultiplexer>().GetDatabase(Configuration.GetRedisDatabaseNumber())
            );

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "MailHole API" });
                var xmlDocPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "MailHole.Api.xml");
                swagger.IncludeXmlComments(xmlDocPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailHole API");
            });

            app.UseMvc();
        }
    }
}