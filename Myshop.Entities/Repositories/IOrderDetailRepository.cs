using Mshop.Entities.Models;
using Mshop.Entities.Repositories;

namespace myshop.Entities.Repositories
{
    public interface IOrderDetailRepository:IGenericRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetail);
    }
}
