using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataLayer.Contexts;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Добавяне на услугите за MVC (Контролери и Изгледи)
            builder.Services.AddControllersWithViews();

            // 2. Регистриране на DbContext с Connection String от appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<FlightManagerDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString) // Pomelo will automatically detect your MySQL version
                ));

            // 3. Регистриране на ASP.NET Core Identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Тук можеш да добавиш изисквания за паролата, ако желаеш
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3; // Пример за по-лесни пароли по време на разработка
            })
            .AddEntityFrameworkStores<FlightManagerDbContext>()
            .AddDefaultTokenProviders();

            // 4. Настройка на FluentEmail (ПРЕМЕСТЕНА ПРЕДИ builder.Build())
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

            // 5. Регистриране на твоите класове (Scoped)
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<FlightContext>();
            builder.Services.AddScoped<ReservationContext>();
            builder.Services.AddScoped<IdentityContext>();

            // Всички builder.Services трябва да са ПРЕДИ този ред:
            var app = builder.Build();

            // Конфигуриране на HTTP request pipeline-а.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // МНОГО ВАЖНО: UseAuthentication() трябва да е ПРЕДИ UseAuthorization()
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}