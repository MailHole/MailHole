using MailHole.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}