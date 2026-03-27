using BusinessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class IdentityContext
    {
        UserManager<User> userManager;
        FlightManagerDbContext context;

        public IdentityContext(FlightManagerDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        #region Seeding Data with this Project

        public async Task SeedDataAsync(string adminPass, string adminEmail)
        {
            //await context.Database.MigrateAsync();

            int userRoles = await context.UserRoles.CountAsync();

            if (userRoles == 0)
            {
                await ConfigureAdminAccountAsync(adminPass, adminEmail);
            }
        }

        public async Task ConfigureAdminAccountAsync(string password, string email)
        {
            User adminIdentityUser = await context.Users.FirstAsync();

            if (adminIdentityUser != null)
            {
                await userManager.AddToRoleAsync(adminIdentityUser, Role.Administrator.ToString());
                await userManager.AddPasswordAsync(adminIdentityUser, password);
                await userManager.SetEmailAsync(adminIdentityUser, email);
            }
        }

        #endregion

        #region CRUD

        public async Task CreateUserAsync(string username, string password, string email, string firstName, string lastName, Role role)
        {
            try
            {
                User user = new User(username, email, firstName, lastName);
                IdentityResult result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new ArgumentException(result.Errors.First().Description);
                }

                if (user.id == 0)
                {
                    await userManager.AddToRoleAsync(user, Role.Administrator.ToString());
                }
                else
                {
                    await userManager.AddToRoleAsync(user, Role.User.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> LogInUserAsync(string username, string password)
        {
            try
            {
                User user = await userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return null;
                }

                bool isPasswordValid = await userManager.CheckPasswordAsync(user, password);
                return isPasswordValid ? user : null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> ReadUserAsync(string key)
        {
            try
            {
                return await userManager.FindByIdAsync(key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> ReadAllUsersAsync()
        {
            try
            {
                return await context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateUserAsync(string id, string username, string firstName, string lastName)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    User user = await context.Users.FindAsync(id);
                    if (user == null) throw new InvalidOperationException("User not found!");
                    user.UserName = username;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    await userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteUserByUsernameAsync(string username)
        {
            try
            {
                User user = await FindUserByNameAsync(username);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found for deletion!");
                }

                await userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByNameAsync(string firstName)
        {
            try
            {
                // Identity return Null if there is no user!
                return await userManager.FindByNameAsync(firstName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUserByLastNameDateAsync(string lastName)
        {
            return await context.Set<User>()
                .Where(u => u.lastName == lastName)
                .OrderBy(u => u.firstName)
                .ToListAsync();
        }

        public async Task<User> GetUserByUserNameDateAsync(string username)
        {
            return await context.Set<User>()
                .Where(u => u.username == username)
                .OrderBy(u => u.firstName)
                .ThenBy(u => u.lastName)
                .ToListAsync();
        }

        public async Task<User> GetUserByEmailDateAsync(string email)
        {
            return await context.Set<User>()
                .Where(u => u.email == email)
                .OrderBy(u => u.firstName)
                .ThenBy(u => u.lastName)
                .ToListAsync();
        }

        #endregion
    }
}

