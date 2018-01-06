using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalXData
{
   public class ShoppingCartInformation
    {
        public int ProductId { get; set; }

        [Display(Name = "Product  Name")]
        public string ProductName { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public List<Address> TheAddresses
        {
            get
            {
                return theAddresses;
            }
            set
            {
                theAddresses = new List<Address>();
            }
        }

        private List<Address> theAddresses;
    }
}
