using FirstInternshipTask.Models;
using Microsoft.EntityFrameworkCore;
using ProductApi.Model;

namespace ProductApi.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<CurrencyRate> CurrencyRates { get; set; }

    }

}
