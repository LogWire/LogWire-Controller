using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model.Application;
using Microsoft.EntityFrameworkCore;

namespace LogWire.Controller.Data.Context
{
    public class ApplicationDataContext : DbContext
    {

        public DbSet<ApplicationEntry> Applications { get; set; }

        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
