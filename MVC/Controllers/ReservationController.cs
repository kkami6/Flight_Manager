using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class ReservationController: Controller
    {
        private readonly ReservationContext _reservationContext;

        public ReservationController(ReservationContext reservationContext)
        {
            _reservationContext = reservationContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _reservationContext.ReadAllAsync();

            var userReservations = reservations
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartTime);

            return View(userReservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string firstName, string middleName, string lastName, 
            string personalId, string phoneNumber, string nationality, string flightType)
        {
            //validations

            var reservation = new ReservationController(firstName, middleName, lastName, personalId, 
                phoneNumber, nationality, flightType);

            try
            {
                await _reservationContext.Create(reservation);
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Write error: " + ex.Message);
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
