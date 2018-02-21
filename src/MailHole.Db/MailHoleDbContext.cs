using MailHole.Db.Entities;
using MailHole.Db.Entities.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MailHole.Db
{
    public class MailHoleDbContext : IdentityDbContext<MailHoleUser, MailHoleRole, string>
    {
        public MailHoleDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Mail>();
            builder.Entity<Attachement>();
        }

        public DbSet<Mail> Mails { get; set; }
    }
}