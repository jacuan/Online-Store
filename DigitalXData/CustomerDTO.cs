using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DigitalXData
{
    public class CustomerDTO
    {
        [System.Web.Mvc.Remote("VerifiyUsernameAvailability", "Membership", ErrorMessage = "Username already exists. Please choose another username", HttpMethod = "GET")]
        [Required]
        [Display(Name = "Username")]
        [RegularExpression(@"^[a-zA-Z].{2,7}$", ErrorMessage = "The username must start with a letter and its length must be between 3 - 8 characters.")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "The {0} must be between 6 - 12 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and the confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int CustomerId { get; set; }
    }
}
