using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataLayer.Contexts;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. MVC
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // 2. DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<FlightManagerDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                ));

            // Identity (single, consistent registration for your custom User)
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<FlightManagerDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                options.SlidingExpiration = true;

                // THIS LINE IS KEY: It allows the cookie to work on localhost without HTTPS
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            // FluentEmail
            var emailSettings = builder.Configuration.GetSection("EmailSettings");
            builder.Services
                .AddFluentEmail(emailSettings["DefaultFrom"])
                .AddSmtpSender(new SmtpClient(emailSettings["SmtpServer"])
                {
                    Port = int.Parse(emailSettings["Port"]),
                    Credentials = new NetworkCredential(
                        emailSettings["Username"],
                        emailSettings["AppPassword"]
                    ),
                    EnableSsl = true
                });

            // Scoped services
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IEmailSender, EmailService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<FlightContext>();
            builder.Services.AddScoped<ReservationContext>();
            builder.Services.AddScoped<IdentityContext>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                // You require parameters here in your signature, even if unused in your current commented-out code
                await identityContext.SeedDataAsync("Admin123!", "admin@domain.com");
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.MapRazorPages();

            app.Run();
        }
    }
}