using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class FlightController: Controller
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string departureLocation, string landingLocation,
            DateTime departureTime, DateTime landingTime, string planeType, string planeId, string PilotName,
            int passengerCapacity, int businessClassCapacity)
        {

            //validate

            var flight = new Flight(departureLocation, landingLocation, departureTime, landingTime, planeType,
                planeId, PilotName, passengerCapacity, businessClassCapacity);

            try
            {
                await _flightContext.Create(flight);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Write error: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _flightContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
