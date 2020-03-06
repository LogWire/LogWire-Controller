using LogWire.Controller.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace LogWire.Controller.Data.Context
{
    public class DataContext : DbContext
    {
        public DbSet<ConfigurationEntry> Configuration { get; set; }
        public DbSet<UserEntry> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
