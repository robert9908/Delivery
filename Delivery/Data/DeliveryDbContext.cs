using Delivery.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Data
{
    public class DeliveryDbContext: DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> dbContextOptions): base(dbContextOptions)
        {
            
        }

        public DbSet<Order> Orders { get; set; }
    }
}
