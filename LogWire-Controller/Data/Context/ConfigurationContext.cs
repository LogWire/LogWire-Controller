using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace LogWire.Controller.Data.Context
{
    public class ConfigurationContext : DbContext
    {
        public DbSet<ConfigurationEntry> Configuration { get; set; }

        public ConfigurationContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
