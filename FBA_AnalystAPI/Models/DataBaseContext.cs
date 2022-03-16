using Microsoft.EntityFrameworkCore;

namespace FBA_AnalystAPI.Models
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
