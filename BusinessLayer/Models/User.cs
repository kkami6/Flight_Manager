using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace BusinessLayer.Models
{
    public enum UserRole { Admin, Employee }
    public class User : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "The personal identification number must be exactly 10 digits.")]
        public string PersonalId { get; set; }

        public string Address { get; set; }

        public UserRole Role { get; set; }
    }
}
