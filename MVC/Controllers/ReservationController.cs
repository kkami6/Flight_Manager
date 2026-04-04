using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DataLayer.Contexts;

namespace MVC.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ReservationContext _reservationContext;

        public ReservationController(ReservationContext reservationContext)
        {
            _reservationContext = reservationContext;
        }

        public async Task<IActionResult> Index(string searchEmail, int page = 1)
        {
            var reservations = await _reservationContext.ReadAllAsync();
            var query = reservations.AsQueryable();

            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(r => r.ContactEmail.Contains(searchEmail, StringComparison.OrdinalIgnoreCase));
            }

            int pageSize = 10;
            var paginatedReservations = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize);
            ViewBag.SearchEmail = searchEmail;

            return View(paginatedReservations);
        }

        public IActionResult Create(int flightId)
        {
            ViewBag.FlightId = flightId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int flightId, string firstName, string middleName, string lastName,
            string personalId, string phoneNumber, string nationality, string flightType, string contactEmail)
        {
            if (string.IsNullOrEmpty(contactEmail) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                ViewBag.FlightId = flightId;
                return View();
            }

            var reservation = new Reservation
            {
                FlightId = flightId,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                PersonalId = personalId,
                PhoneNumber = phoneNumber,
                Nationality = nationality,
                FlightType = flightType,
                ContactEmail = contactEmail
            };

            try
            {
                await _reservationContext.CreateAsync(new List<Reservation> { reservation });

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Write error: " + ex.Message);
                ViewBag.FlightId = flightId;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _reservationContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}