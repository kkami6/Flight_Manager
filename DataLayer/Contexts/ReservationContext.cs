using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class ReservationContext : IDB<Reservation, int>
    {
        private readonly FlightManagerDbContext context;
        private readonly IEmailService _emailService;

        public ReservationContext(FlightManagerDbContext context, IEmailService emailService)
        {
            this.context = context;
            _emailService = emailService;
        }

        public async Task CreateAsync(IEnumerable<Reservation> items)
        {
            try
            {
                var flightId = items.First().FlightId;
                var flight = context.Set<Flight>().FirstOrDefault(f => f.Id == flightId);
                if (flight != null)
                {
                    int requestedBusinessSeats = items.Count(r => r.FlightType == "Business");
                    int requestedRegularSeats = items.Count(r => r.FlightType == "Regular");

                    if (flight.BusinessClassCapacity >= requestedBusinessSeats && flight.PassengerCapacity >= requestedRegularSeats)
                    {
                        foreach (var item in items)
                        {
                            context.Set<Reservation>().Add(item);

                            if (item.FlightType == "Business")
                            {
                                flight.BusinessClassCapacity--;
                            }
                            else
                            {
                                flight.PassengerCapacity--;
                            }

                            await _emailService.SendReservationConfirmationAsync(item.ContactEmail, item.FlightId, item.Id);
                        }

                        // Save changes only if everything is valid
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        // Handle the case where there aren't enough seats
                        throw new InvalidOperationException("Not enough seats available for the selected ticket types.");
                    }
                }
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task CreateAsync(Reservation item)
        {
            await CreateAsync(new List<Reservation> { item });
        }

        public async Task<Reservation> ReadAsync(int key)
        {
            try
            { return await context.Set<Reservation>().FirstOrDefaultAsync(r => r.Id == key); }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<Reservation>> ReadAllAsync()
        {
            try
            {
                // We remove the OrderBy for a second to see if the data appears
                return await context.Set<Reservation>()
                    .Include(r => r.Flight)
                    .ToListAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task UpdateAsync(Reservation item)
        {
            try
            {
                context.Set<Reservation>().Update(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task DeleteAsync(int key)
        {
            try
            {
                var reservation = await context.Set<Reservation>().FindAsync(key);

                if (reservation == null)
                    throw new ArgumentException("reservation not found.");

                context.Set<Reservation>().Remove(reservation);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByDateAsync(string email)
        {
            return await context.Set<Reservation>()
                .Where(r => r.ContactEmail == email)
                .OrderBy(r => r.FirstName)
                .ThenBy(r => r.LastName)
                .ToListAsync();
        }

    }
}

