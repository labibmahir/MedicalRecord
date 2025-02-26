using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Hospital> Hospitals { get; set; }

        public DbSet<HospitalAccess> HospitalAccess { get; set; }

        public DbSet<Patient> Patients { get; set; }
    }
}
