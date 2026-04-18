using BusinessLayer.Models;
using DataLayer.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.ViewModels;

namespace MVC.Controllers
{
    public class FlightController : Controller
    {
        private readonly FlightContext _flightContext;
        private readonly ReservationContext _reservationContext;

        public FlightController(FlightContext flightContext, ReservationContext reservationContext)
        {
            _flightContext = flightContext;
            _reservationContext = reservationContext;
        }

        public async Task<IActionResult> Index()
        {
            var flights = await _flightContext.ReadAllAsync();
            return View(flights);
        }

        public async Task<IActionResult> Details(int id, int page = 1)
        {
            // IMPROVEMENT: Use a method that finds ONE flight by ID, not all of them
            var flight = await _flightContext.ReadAsync(id);

            if (flight == null) return NotFound();

            int pageSize = 10;
            var reservations = flight.Reservations ?? new List<Reservation>();

            var paginatedReservations = reservations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(reservations.Count / (double)pageSize);
            ViewBag.Reservations = paginatedReservations;

            return View(flight);
        }

        [Authorize(Roles = "Admin")] // Only Admins can enter
        public async Task<IActionResult> AdminIndex()
        {
            // Fetch all flights
            var flights = await _flightContext.ReadAllAsync();
            // Fetch all reservations to link them
            var allReservations = await _reservationContext.ReadAllAsync();

            var viewModel = flights.Select(f => new AdminFlightViewModel
            {
                FlightId = f.Id,
                DepartureLocation = f.DepartureLocation,
                LandingLocation = f.LandingLocation,
                DepartureTime = f.DepartureTime,
                LandingTime = f.LandingTime,
                TotalCapacity = f.PassengerCapacity + f.BusinessClassCapacity,
                // Filter reservations belonging to THIS flight
                Passengers = allReservations.Where(r => r.FlightId == f.Id).ToList()
            }).ToList();

            return View(viewModel);
        }

        // GET: Flight/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _flightContext.ReadAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flight/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (flight.LandingTime <= flight.DepartureTime)
            {
                ModelState.AddModelError("LandingTime", "Landing time must be after departure time.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _flightContext.UpdateAsync(flight);
                    return RedirectToAction(nameof(AdminIndex)); // Send back to Admin dashboard
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes: " + ex.Message);
                }
            }
            return View(flight);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        [ValidateAntiForgeryToken]
        // IMPROVEMENT: Bind directly to the Flight object
        public async Task<IActionResult> Create(Flight flight)
        {
            // Check logic validation
            if (flight.LandingTime <= flight.DepartureTime)
            {
                ModelState.AddModelError("LandingTime", "Landing time must be after departure time.");
            }

            // This checks the [Required] and [Range] attributes in your Flight class
            if (ModelState.IsValid)
            {
                try
                {
                    await _flightContext.CreateAsync(flight);
                    return RedirectToAction(nameof(Create));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.Message);
                }
            }

            return View(flight);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        [ValidateAntiForgeryToken] // Added for security
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _flightContext.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                // Optionally handle delete errors (e.g. flight has reservations)
                TempData["Error"] = "Could not delete flight: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}