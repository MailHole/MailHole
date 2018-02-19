using System;
using System.IO;
using Hangfire;
using Hangfire.Redis;
using MailHole.Api.Auth;
using MailHole.Api.Hangfire;
using MailHole.Common.Extensions;
using MailHole.Common.HangfireExtensions;
using MailHole.Common.Model.Options;
using MailHole.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            
            services
                .ConfigureMailHoleOptions(Configuration)
                .AddJobScheduler(Configuration)
                .AddApiDoc();

            services.AddDbContext<MailHoleDbContext>(options =>
            {
                options.UseNpgsql("User ID=mailhole;Password=*h*#XNSsh5nxw,,OnTcb1-|1@U2NX!;Host=localhost;Port=5432;Database=MailHole;Pooling=true;", builder => { builder.MigrationsAssembly(typeof(MailHoleDbContext).Assembly.GetName().Name); });
            });

            services.AddTokenAuth(Configuration.BindAuthOptions());
            
            services.TryAddTransient(provider =>
            {
                var minioOptions = provider.GetMinioOptionsOrDefault();
                return new MinioClient(minioOptions.Endpoint, minioOptions.AccessKey, minioOptions.SecretKey, minioOptions.Region);
            });
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<MailHoleDbContext>()?.Database.Migrate();
                scope.ServiceProvider.GetService<AuthSeeder>().SeedAdminUser().Wait();
            }
            app.UseAuthentication();
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Activator = new HangfireActivator(serviceProvider)
            });
            
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new[] {new HangfireAuthFilter()}
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailHole API"); });
            app.UseMvc();
        }
    }
}