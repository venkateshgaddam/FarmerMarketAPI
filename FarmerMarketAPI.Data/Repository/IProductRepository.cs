using FarmerMarketAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Data.Repository
{
    public interface IProductRepository
    {
        Product GetProductByCode(string productCode);


        Task<List<Product>> GetProducts();
    }
}
