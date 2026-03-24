using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DataLayer.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Contexts
{
    public class FlightManagerDbContext : IdentityDbContext<User>
    {
    public FlightManagerDbContext(DbContextOptions<FlightManagerDbContext> options) : base(options)
    {

    }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

    }

       protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new FlightConfiguration());
            builder.ApplyConfiguration(new ReservationConfiguration());

        }


    }
