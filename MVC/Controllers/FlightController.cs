using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DataLayer.Contexts;
using Microsoft.AspNetCore.Authorization;

namespace MVC.Controllers
{
    public class FlightController : Controller
    {
        private readonly FlightContext _flightContext;

        public FlightController(FlightContext flightContext)
        {
            _flightContext = flightContext;
        }

        public async Task<IActionResult> Index()
        {
            var flights = await _flightContext.ReadAllAsync();
            return View(flights);
        }

        public async Task<IActionResult> Details(int id, int page = 1)
        {
            var flights = await _flightContext.ReadAllAsync();
            var flight = flights.FirstOrDefault(f => f.Id == id);

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

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string departureLocation, string landingLocation,
            DateTime departureTime, DateTime landingTime, string planeType, string planeId, string pilotName,
            int passengerCapacity, int businessClassCapacity)
        {
            if (landingTime <= departureTime)
            {
                ModelState.AddModelError("", "Landing time must be after departure time.");
                return View();
            }

            var flight = new Flight
            {
                DepartureLocation = departureLocation,
                LandingLocation = landingLocation,
                DepartureTime = departureTime,
                LandingTime = landingTime,
                PlaneType = planeType,
                PlaneId = planeId,
                PilotName = pilotName,
                PassengerCapacity = passengerCapacity,
                BusinessClassCapacity = businessClassCapacity
            };

            try
            {
                await _flightContext.CreateAsync(flight);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Write error: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            await _flightContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}