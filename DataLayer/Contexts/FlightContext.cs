using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace DataLayer.Contexts
{
    public class HolidayContext : IDB<Holiday, int>
    {
        private readonly PlannerDbContext context;

        public HolidayContext(PlannerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(Holiday item)
        {
            try
            {
                context.Set<Holiday>().Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<Holiday> ReadAsync(int key)
        {
            try
            { return await context.Set<Holiday>().FirstOrDefaultAsync(h => h.ActivityId == key); }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task<IEnumerable<Holiday>> ReadAllAsync()
        {
            try
            { return await context.Set<Holiday>().OrderBy(h => h.Date).ToListAsync(); }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task UpdateAsync(Holiday item)
        {
            try
            {
                context.Set<Holiday>().Update(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        public async Task DeleteAsync(int key)
        {
            try
            {
                var holiday = await context.Set<Holiday>().FindAsync(key);

                if (holiday == null)
                    throw new ArgumentException("Holiday not found.");

                context.Set<Holiday>().Remove(holiday);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }


