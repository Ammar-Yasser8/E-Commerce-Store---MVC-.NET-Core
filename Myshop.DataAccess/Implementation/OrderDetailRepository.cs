using Mshop.DataAccess.Implementation;
using Mshop.DataAccess;
using Mshop.Entities.Models;
using Mshop.Entities.Repositories;

namespace myshop.Entities.Repositories
{
	public class OrderDetailRepository : GenericRepository<OrderDetails>, IOrderDetailRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderDetailRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderDetails orderDetails)
		{
			_context.orderDetails.Update(orderDetails);			
		}
	}
}
