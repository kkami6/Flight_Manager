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
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.MiddleName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.PersonalId)
                   .IsRequired()
                   .HasMaxLength(10); // EGN is exactly 10 digits

            builder.Property(r => r.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(r => r.Nationality)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.FlightType)
                   .IsRequired()
                   .HasMaxLength(50); // E.g., "Regular" or "Business"

            builder.Property(r => r.ContactEmail)
                   .IsRequired()
                   .HasMaxLength(256);

            // Ignore the calculated FullName property so EF doesn't try to map it
            builder.Ignore(r => r.FullName);
        }
    }
}
