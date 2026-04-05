using BusinessLayer.Models;
using DataLayer.Contexts;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FlightManagerDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Identity (single consistent registration for your custom User)
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FlightManagerDbContext>();

// Optional: role-based Identity instead of AddDefaultIdentity
// builder.Services.AddIdentity<User, IdentityRole>(options => { /* password options */ })
//     .AddEntityFrameworkStores<FlightManagerDbContext>()
//     .AddDefaultTokenProviders();

// Email / other services
var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddFluentEmail(emailSettings["DefaultFrom"])
    .AddSmtpSender(new SmtpClient(emailSettings["SmtpServer"])
    {
        Port = int.Parse(emailSettings["Port"]),
        Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["AppPassword"]),
        EnableSsl = true
    });

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<FlightContext>();
builder.Services.AddScoped<ReservationContext>();
builder.Services.AddScoped<IdentityContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();