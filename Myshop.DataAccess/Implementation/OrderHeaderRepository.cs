using myshop.Entities.Repositories;
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
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;

		}

		public void Update(OrderHeader orderHeader )
		{
			_context.orderHeaders.Update( orderHeader );
		}

		public void UpdateStatus(int id, string? OrderStatus, string? PaymentStatus)
		{
			var orderfromDb = _context.orderHeaders.FirstOrDefault( x => x.Id == id );
            if (orderfromDb != null)
            {
                orderfromDb.OrderStatus=OrderStatus;
				if (PaymentStatus != null)
				{
					orderfromDb.PaymentStatus=PaymentStatus;
				}

            }
        }
	}
}
