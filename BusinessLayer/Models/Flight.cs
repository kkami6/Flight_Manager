using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        public string DepartureLocation { get; set; }

        [Required]
        public string LandingLocation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime LandingTime { get; set; }

        [Required]
        public string PlaneType { get; set; }

        [Required]
        public string PlaneId { get; set; } 

        [Required]
        public string PilotName { get; set; }

        [Range(1, 1000)]
        public int PassengerCapacity { get; set; }

        [Range(0, 500)]
        public int BusinessClassCapacity { get; set; }

        public TimeSpan Duration => LandingTime - DepartureTime;

        // Adding the ? tells the system this doesn't have to come from the form
        public virtual ICollection<Reservation>? Reservations { get; set; } = new List<Reservation>();
    }
}
