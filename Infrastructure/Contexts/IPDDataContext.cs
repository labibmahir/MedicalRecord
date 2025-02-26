using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contexts
{
    public class IPDDataContext : DbContext
    {
        public IPDDataContext(DbContextOptions<IPDDataContext> options) : base(options)
        {

        }

        public DbSet<IPDPatient> IPDPatients { get; set; }

        public DbSet<IPDRecord> IPDRecords { get; set; }
    }
}
