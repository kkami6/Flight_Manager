using BusinessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Models;
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
        private readonly UserManager<User> _userManager;
        private readonly FlightManagerDbContext _context;
        public IdentityContext(FlightManagerDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        #region Seeding Data with this Project

        public async Task SeedDataAsync(string adminPass, string adminEmail)
        {
            //await context.Database.MigrateAsync();

            int userRoles = await _context.UserRoles.CountAsync();

            if (userRoles == 0)
            {
                await ConfigureAdminAccountAsync(adminPass, adminEmail);
            }
        }

        public async Task ConfigureAdminAccountAsync(string password, string email)
        {
            User adminIdentityUser = await _context.Users.FirstAsync();

            if (adminIdentityUser != null)
            {
                await _userManager.AddToRoleAsync(adminIdentityUser, UserRole.Admin.ToString());
                await _userManager.AddPasswordAsync(adminIdentityUser, password);
                await _userManager.SetEmailAsync(adminIdentityUser, email);
            }
        }

        #endregion

        #region CRUD

        public async Task CreateUserAsync(string username, string password, string email, string firstName, string lastName, string personalId, string address,
    string phoneNumber, UserRole role)
        {
            try
            {
                User user = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PersonalId = personalId,
                    Address = address,
                    PhoneNumber = phoneNumber
                };
                IdentityResult result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new ArgumentException(result.Errors.First().Description);
                }

                var adminUsers = await _userManager.GetUsersInRoleAsync(UserRole.Admin.ToString());

                if (!adminUsers.Any())
                {
                    await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, UserRole.Employee.ToString());
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
                User user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return null;
                }

                bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
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
                return await _userManager.FindByIdAsync(key);
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
                return await _context.Users.ToListAsync();
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
                    User user = await _context.Users.FindAsync(id);
                    if (user == null) throw new InvalidOperationException("User not found!");
                    user.UserName = username;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    await _userManager.UpdateAsync(user);
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

                await _userManager.DeleteAsync(user);
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
                return await _userManager.FindByNameAsync(firstName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<User>> GetUserByLastNameDateAsync(string lastName)
        {
            return await _context.Set<User>()
                .Where(u => u.LastName == lastName)
                .OrderBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUserByUserNameDateAsync(string username)
        {
            return await _context.Set<User>()
                .Where(u => u.UserName == username)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUserByEmailDateAsync(string email)
        {
            return await _context.Set<User>()
                .Where(u => u.Email == email)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        #endregion
    }
}

