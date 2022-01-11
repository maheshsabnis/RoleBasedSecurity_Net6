using Microsoft.EntityFrameworkCore;
using SharedModels.Models;

namespace RoleBasedSecurity_Net6.Models
{
    public class OrdersDbContext : DbContext
    {
        public DbSet<Orders> Orders { get; set; }
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
