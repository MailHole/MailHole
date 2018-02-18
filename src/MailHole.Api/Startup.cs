using System;
using System.IO;
using Hangfire;
using Hangfire.Redis;
using MailHole.Api.Hangfire;
using MailHole.Common.Extensions;
using MailHole.Common.HangfireExtensions;
using MailHole.Common.Model.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Minio;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;

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
            var hangfireOptions = Configuration.BindHangfireOptions();
            services.AddMvc();
            services.ConfigureMailHoleOptions(Configuration);
            services.AddHangfire(config =>
                {
                    config.UseRedisStorage(hangfireOptions.RedisConnectionString, new RedisStorageOptions
                    {
                        Db = hangfireOptions.RedisDatabaseIndex,
                        Prefix = hangfireOptions.RedisPrefix
                    });
                });
            
            services.TryAddSingleton<IConnectionMultiplexer>(provier =>
                ConnectionMultiplexer.Connect(provier.GetRedisOptionsOrDefault().ConnectionString)
            );
            
            services.TryAddTransient(provider =>
            {
                var minioOptions = provider.GetMinioOptionsOrDefault();
                return new MinioClient(minioOptions.Endpoint, minioOptions.AccessKey, minioOptions.SecretKey, minioOptions.Region);
            });
            
            services.TryAddTransient<IDatabaseAsync>(provider =>
                provider.GetService<IConnectionMultiplexer>().GetDatabase(provider.GetRedisOptionsOrDefault().DatabaseIndex)
            );

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new Info { Title = "MailHole API" });
                var xmlDocPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, $"{typeof(Startup).Assembly.GetName().Name}.xml");
                swagger.IncludeXmlComments(xmlDocPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new [] { new HangfireAuthFilter()}
            });
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Activator = new HangfireActivator(serviceProvider)
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