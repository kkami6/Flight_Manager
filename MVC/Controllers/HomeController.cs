using DataLayer.Contexts;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.ViewModels;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ReservationContext _reservationContext;

        public HomeController(ILogger<HomeController> logger, ReservationContext reservationContext)
        {
            _logger = logger;
            _reservationContext = reservationContext;
        }

        public async Task<IActionResult> Index(string searchEmail, int page = 1)
        {
            // 1. Fetch data
            var allReservations = await _reservationContext.ReadAllAsync();

            if (allReservations == null || !allReservations.Any())
            {
                return View(new List<BulkReservationSummaryViewModel>());
            }

            // 2. Grouping Logic
            var groupedReservations = allReservations
                .GroupBy(r => new { r.ContactEmail, r.FlightId })
                .Select(g => new BulkReservationSummaryViewModel
                {
                    ContactEmail = g.Key.ContactEmail ?? "No Email",
                    FlightId = g.Key.FlightId,
                    PassengerCount = g.Count(),
                    Departure = g.First().Flight != null ? g.First().Flight.DepartureLocation : "N/A",
                    Landing = g.First().Flight != null ? g.First().Flight.LandingLocation : "N/A",
                    GroupMembers = g.ToList()
                })
                .ToList();

            // 3. Search Filter
            if (!string.IsNullOrWhiteSpace(searchEmail))
            {
                groupedReservations = groupedReservations
                    .Where(g => g.ContactEmail.Contains(searchEmail, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // 4. Pagination
            int pageSize = 3;
            var paginatedGroups = groupedReservations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(groupedReservations.Count / (double)pageSize);
            ViewBag.SearchEmail = searchEmail;

            return View(paginatedGroups);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}