using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalXData
{
    public class OrderedItem
    {
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Completed")]
        public bool Complete { get; set; }

        [Display(Name = "Back Order")]
        public bool IsBackOrder { get; set; }

        public Product Product { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
