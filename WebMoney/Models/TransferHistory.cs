namespace WebMoney.Models
{
    public class TransferHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ReplenishId { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType Type { get; set; }
        public int Amount { get; set; }
        public int? ReceiverId { get; set; }
        public User User { get; set; }
        public Replenish Replenish { get; set; }
        public User Receiver { get; set; }
    }

    public enum TransactionType
{
    Replenish,
    Transfer,
    Withdraw
}
}
