using System.ComponentModel.DataAnnotations.Schema;

namespace FBA_AnalystAPI.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public int UserId { get; set; }
    }
}
