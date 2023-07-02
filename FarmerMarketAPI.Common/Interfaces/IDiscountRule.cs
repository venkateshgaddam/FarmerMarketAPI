using FarmerMarketAPI.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Common.Interfaces
{
    public interface IDiscountRule
    {
        bool IsApplicable(Dictionary<string, int> basket);
        decimal CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository);
    }

}
