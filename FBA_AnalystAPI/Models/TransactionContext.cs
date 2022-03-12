using Microsoft.EntityFrameworkCore;

namespace FBA_AnalystAPI.Models
{
    public class TransactionContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public TransactionContext(DbContextOptions<TransactionContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
