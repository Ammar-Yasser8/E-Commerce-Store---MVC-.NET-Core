using Mshop.Entities.Model;
using Mshop.Entities.Models;
using Mshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.DataAccess.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
             
        }

        public Task GetAllAsync(Func<object, object> value)
        {
            throw new NotImplementedException();
        }

        public void Update(Product product)
        {
             var productInDb = _context.Products.FirstOrDefault(x=> x.Id == product.Id);
            if (productInDb != null)
            {
                productInDb.Name= product.Name;
                productInDb.Description= product.Description;
                productInDb.Price= product.Price;
                productInDb.Img= product.Img;
                productInDb.CategoryId= product.CategoryId; 
            }
        }
    }
}
