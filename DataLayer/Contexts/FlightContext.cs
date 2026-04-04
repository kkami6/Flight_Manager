using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace DataLayer.Contexts
{
    public class FlightContext : IDB<Flight, int>
    {
        private readonly FlightManagerDbContext context;

        public FlightContext(FlightManagerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(Flight item)
        {
            try
            {
                context.Set<Flight>().Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<Flight> ReadAsync(int key)
        {
            try
            { return await context.Set<Flight>().FirstOrDefaultAsync(f => f.Id == key); }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<Flight>> ReadAllAsync()
        {
            try
            { return await context.Set<Flight>().OrderBy(f => f.DepartureTime).ToListAsync(); }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task UpdateAsync(Flight item)
        {
            try
            {
                context.Set<Flight>().Update(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task DeleteAsync(int key)
        {
            try
            {
                var flight = await context.Set<Flight>().FindAsync(key);

                if (flight == null)
                    throw new ArgumentException("flight not found.");

                context.Set<Flight>().Remove(flight);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<Flight>> GetFlightAfterDateAsync(DateOnly date)
        {
            return await context.Set<Flight>()
                .Where(f => DateOnly.FromDateTime(f.DepartureTime) >= date)
                .OrderBy(f => f.DepartureTime)
                .ToListAsync();
        }

    }
}