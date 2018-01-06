using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalXData
{
    public class CustomerAddress
    {
        [Required]
        [Display(Name = "Address Type")]
        public int AddressType { get; set; }

        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Suburb")]
        public string Suburb { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Post Code")]
        [StringLength(10, ErrorMessage = "The maximum length of characters for postal code is 10.")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public int AddressId { get; set; }
    }
}
