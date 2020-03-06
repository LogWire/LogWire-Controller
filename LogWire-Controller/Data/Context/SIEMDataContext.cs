using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogWire.Controller.Data.Model.SIEM;
using Microsoft.EntityFrameworkCore;

namespace LogWire.Controller.Data.Context
{
    public class SIEMDataContext : DbContext
    {

        public DbSet<SIEMUserEntry> Users { get; set; }

        public SIEMDataContext(DbContextOptions<SIEMDataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
