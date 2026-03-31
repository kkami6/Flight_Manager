using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid personal identification number.")]
        public string PersonalId { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public string FlightType { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; } 

        public int FlightId { get; set; }

        public virtual Flight Flight { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}";
    }
}
