using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace MailHole.Db
{
    public class MailHoleDbContextFactory : IDesignTimeDbContextFactory<MailHoleDbContext>
    {
        public MailHoleDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("appsettings.json")
                .Build();
            
            var options = new DbContextOptionsBuilder()
                .UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .Options;
            
            return new MailHoleDbContext(options);
        }
    }
}