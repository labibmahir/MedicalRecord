using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contexts
{
    public class OPDDataContext : DbContext
    {
        public OPDDataContext(DbContextOptions<OPDDataContext> options) : base(options)
        {

        }

        public DbSet<OPDPatient> OPDPatients { get; set; }

        public DbSet<OPDRecord> OPDRecords { get; set; }
    }
}