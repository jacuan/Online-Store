using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalXData
{
    public class ProductDTO
    {
        [Display(Name = "Product  Name")]
        public string ProductName { get; set; }

        [Display(Name = "Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Unit Price")]
        public decimal Price { get; set; }

        [Display(Name = "Units in Stock")]
        public int UnitsInStock { get; set; }

        public byte[] Picture { get; set; }

        public int ProductId { get; set; }
    }
}
