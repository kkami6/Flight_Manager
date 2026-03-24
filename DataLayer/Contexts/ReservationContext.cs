using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace DataLayer.Contexts
{
    public class ReservationContext : IDB<Reservation, int>
    {
        private readonly FlightManagerDbContext context;

        public ReservationContext(FlightManagerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(Reservation item)
        {
            try
            {
                context.Set<Reservation>().Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<Reservation> ReadAsync(int key)
        {
            try
            { return await context.Set<Reservation>().FirstOrDefaultAsync(r => r.ActivityId == key); }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<Reservation>> ReadAllAsync()
        {
            try
            { return await context.Set<Reservation>().OrderBy(r => r.Date).ToListAsync(); }
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


