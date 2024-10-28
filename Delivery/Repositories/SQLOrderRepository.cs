using Delivery.Data;
using Delivery.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Repositories
{
    public class SQLOrderRepository: IOrderRepository
    {
        private readonly DeliveryDbContext dbContext;

        public SQLOrderRepository(DeliveryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Order> CreateAsync(Order order)
        {
           await dbContext.Orders.AddAsync(order);
           await dbContext.SaveChangesAsync();
           return order;   
        }

        public async Task<Order?> DeleteAsync(int id)
        {
            var existingOrder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (existingOrder == null) { return null; }

            dbContext.Orders.Remove(existingOrder);
            await dbContext.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<List<Order>> GetAllAsync(DateTime? fromDate = null, string? district = null)
        {
            var orders = dbContext.Orders.AsQueryable();

            if (fromDate.HasValue)
            {
                DateTime toDate = fromDate.Value.AddMinutes(30);

                orders = orders.Where(x => x.DeliveryDateTime >= fromDate.Value && x.DeliveryDateTime <= toDate);
            }
            if (!string.IsNullOrWhiteSpace(district))
            {
                orders = orders.Where(x => x.District == district);
            }

            return await orders.ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> UpdateAsync(int id, Order order)
        {
            var existingOrder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (existingOrder == null) { return null; }

            existingOrder.Name = order.Name;
            existingOrder.Weight = order.Weight;
            existingOrder.District = order.District;
            existingOrder.DeliveryDateTime = order.DeliveryDateTime;

            await dbContext.SaveChangesAsync();
            return existingOrder;
        }
    }
}
