using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace MVC.ViewModels
{
    public class BulkReservationViewModel
    {
        [Required(ErrorMessage = "Please select a flight")]
        public int FlightId { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        public string FlightType { get; set; } // "Regular" or "Business"

        // This list will hold the details for every passenger in the group
        public List<PassengerDetail> Passengers { get; set; } = new List<PassengerDetail>();
    }

    public class PassengerDetail
    {
        [Required] public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string PersonalId { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required] public string Nationality { get; set; }
    }
}