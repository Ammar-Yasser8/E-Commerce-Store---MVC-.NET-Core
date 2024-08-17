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
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>,IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
             
        }

        public ShoppingCart Decreasecount(ShoppingCart shoppingCart, int count)
        {
           shoppingCart.Count -= count;
            return shoppingCart;
        }

        public ShoppingCart Increasecount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart;
        }
    }
}         
