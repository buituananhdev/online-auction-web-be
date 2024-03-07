using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Domain
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configration;
        public DataContext(DbContextOptions<DataContext> options, IConfiguration configration) : base(options)
        {
            _configration = configration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
