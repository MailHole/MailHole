using System.IO;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Redis;
using MailHole.Api.Hangfire;
using MailHole.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Minio;
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
            services.AddHangfire(config =>
                {
                    config.UseRedisStorage(Configuration.GetRedisConnectionString(), new RedisStorageOptions
                    {
                        Db = Configuration.GetRedisDatabaseNumber(),
                        Prefix = "Hangfire"
                    });
                });
            
            services.TryAddSingleton<IConnectionMultiplexer>(provier =>
                ConnectionMultiplexer.Connect(Configuration.GetRedisConnectionString())
            );
            
            services.TryAddTransient(provider => new MinioClient("minio:9000", "19OWL0F004QOXHLK8KN4",
                "LVUTKrQgRVY2iZr9iIArdGpXqvSA4gZqxWsif8U8"));
            
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
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new [] { new HangfireAuthFilter()}
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailHole API");
            });

            app.UseMvc();
        }
    }
}