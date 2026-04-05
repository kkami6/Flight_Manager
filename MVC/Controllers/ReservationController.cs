using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DataLayer.Contexts;
using MVC.ViewModels; // Ensure this matches your ViewModel namespace

namespace MVC.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ReservationContext _reservationContext;
        private readonly FlightContext _flightContext; // Added this

        // Updated Constructor
        public ReservationController(ReservationContext reservationContext, FlightContext flightContext)
        {
            _reservationContext = reservationContext;
            _flightContext = flightContext;
        }

        //public async Task<IActionResult> Index(string searchEmail, int page = 1)
        //{
        //    // 1. Get everything from the context
        //    var allReservations = await _reservationContext.ReadAllAsync();

        //    // 2. If the list is null or empty here, the ReadAllAsync isn't fetching the data you just created
        //    if (allReservations == null || !allReservations.Any())
        //    {
        //        return View(new List<BulkReservationSummaryViewModel>());
        //    }

        //    var query = allReservations.AsQueryable();

        //    // 3. Search filter (Safe for nulls)
        //    if (!string.IsNullOrWhiteSpace(searchEmail))
        //    {
        //        query = query.Where(r => r.ContactEmail != null &&
        //                                 r.ContactEmail.Contains(searchEmail, StringComparison.OrdinalIgnoreCase));
        //    }

        //    // 4. Grouping: We use "Default" values to prevent rows from being dropped
        //    var groupedReservations = query
        //        .GroupBy(r => new {
        //            Email = r.ContactEmail ?? "No Email Provided",
        //            FId = r.FlightId
        //        })
        //        .ToList() // Materialize the query so we can use null-propagation in memory
        //        .Select(g => new BulkReservationSummaryViewModel
        //        {
        //            ContactEmail = g.Key.Email,
        //            FlightId = g.Key.FId,
        //            PassengerCount = g.Count(),
        //            Departure = g.FirstOrDefault()?.Flight?.DepartureLocation ?? "Location N/A",
        //            Landing = g.FirstOrDefault()?.Flight?.LandingLocation ?? "Location N/A",
        //            GroupMembers = g.ToList()
        //        })
        //        .ToList();

        //    // 5. Pagination
        //    int pageSize = 3;
        //    var paginatedGroups = groupedReservations
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = (int)Math.Ceiling(groupedReservations.Count / (double)pageSize);
        //    ViewBag.SearchEmail = searchEmail;

        //    return View(paginatedGroups);
        //}

        // GET: Displays the form with the Flight Dropdown
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Get flights that haven't departed yet
            var allFlights = await _flightContext.ReadAllAsync();
            var availableFlights = allFlights.Where(f => f.DepartureTime > DateTime.Now);

            // Create the SelectList for the dropdown
            ViewBag.Flights = new SelectList(availableFlights, "Id", "DepartureLocation");

            // Return the view with an empty ViewModel
            return View(new BulkReservationViewModel());
        }

        // POST: Processes the multiple passengers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BulkReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Convert the list of PassengerDetails into a list of Reservation objects
                var reservations = model.Passengers.Select(p => new Reservation
                {
                    FlightId = model.FlightId,
                    ContactEmail = model.ContactEmail,
                    FlightType = model.FlightType,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName ?? "", // Handle optional middle name
                    LastName = p.LastName,
                    PersonalId = p.PersonalId,
                    PhoneNumber = p.PhoneNumber,
                    Nationality = p.Nationality
                }).ToList();

                try
                {
                    // Call your context method that handles the IEnumerable
                    await _reservationContext.CreateAsync(reservations);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Booking Error: " + ex.Message);
                }
            }

            // If we get here, something failed. Reload the dropdown and return the view.
            var allFlights = await _flightContext.ReadAllAsync();
            ViewBag.Flights = new SelectList(allFlights.Where(f => f.DepartureTime > DateTime.Now), "Id", "DepartureLocation");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _reservationContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}