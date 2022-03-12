namespace FBA_AnalystAPI.Models
{
    public class Transaction
    {
        public int id { get; set; }
        public User User { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
    }
}
