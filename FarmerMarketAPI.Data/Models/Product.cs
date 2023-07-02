using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Data.Models
{
    public class Product
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
