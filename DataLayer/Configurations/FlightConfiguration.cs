using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Configurations
{
    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.ToTable("Flights");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.DepartureLocation)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(f => f.LandingLocation)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(f => f.DepartureTime)
                   .IsRequired();

            builder.Property(f => f.LandingTime)
                   .IsRequired();

            builder.Property(f => f.PlaneType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(f => f.PlaneId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(f => f.PilotName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(f => f.PassengerCapacity)
                   .IsRequired();

            builder.Property(f => f.BusinessClassCapacity)
                   .IsRequired();

            builder.Ignore(f => f.Duration);

            builder.HasMany(f => f.Reservations)
                   .WithOne(r => r.Flight)
                   .HasForeignKey(r => r.FlightId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
