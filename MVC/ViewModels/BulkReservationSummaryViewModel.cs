using BusinessLayer.Models;

namespace MVC.ViewModels
{
    public class BulkReservationSummaryViewModel
    {
        public string ContactEmail { get; set; }
        public int FlightId { get; set; }
        public int PassengerCount { get; set; }
        public string Departure { get; set; }
        public string Landing { get; set; }
        public List<Reservation> GroupMembers { get; set; } = new List<Reservation>();
    }
}