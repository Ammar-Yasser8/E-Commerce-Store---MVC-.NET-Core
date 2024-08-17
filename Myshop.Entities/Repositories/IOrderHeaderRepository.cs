using Mshop.Entities.Models;
using Mshop.Entities.Repositories;

namespace myshop.Entities.Repositories
{
	public interface IOrderHeaderRepository:IGenericRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int id, string? OrderStatus, string? PaymentStatus);
    }
}
