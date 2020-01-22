using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Untappd.KolosBot.Infrastucture
{
    public class KolosDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }

    public class User
    {
        public long Id { get; set; }
    }
}
