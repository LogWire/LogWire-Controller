using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace LogWire.Controller.Data.Context
{
    public class DataContext : DbContext
    {
        public DbSet<ConfigurationEntry> Configuration { get; set; }
        public DbSet<UserEntry> Users { get; set; }

        public DataContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
