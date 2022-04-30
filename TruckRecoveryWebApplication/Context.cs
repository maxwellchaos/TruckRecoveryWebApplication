using Microsoft.EntityFrameworkCore;
using WebServiceTruckRecovery.Models;

namespace TruckRecoveryWebApplication
{
    public class Context : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<SystemUser> Users { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<WebServiceTruckRecovery.Models.Client> Client { get; set; }
    }
}
