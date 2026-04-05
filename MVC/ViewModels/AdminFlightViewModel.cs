namespace MVC.ViewModels
{
    public class AdminFlightViewModel
    {
        public int FlightId { get; set; }
        public string DepartureLocation { get; set; }
        public string LandingLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime LandingTime { get; set; }
        public int TotalCapacity { get; set; }

        // Calculated Property for Duration
        public string Duration => (LandingTime - DepartureTime).ToString(@"hh\:mm") + " hours";

        public List<BusinessLayer.Models.Reservation> Passengers { get; set; } = new();
    }
}