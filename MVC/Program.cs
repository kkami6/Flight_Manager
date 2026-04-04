using BusinessLayer.Interfaces;
using FluentEmail.Smtp;
using System.Net;
using System.Net.Mail;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

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

            // 3. Register your custom service
            builder.Services.AddScoped<IEmailService, EmailService>();

            app.Run();
        }
    }
}
