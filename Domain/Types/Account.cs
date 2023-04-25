namespace TransactionService.Domain.Types;

public class Account: Base {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int CurrencyId { get; set; }
    public double Amount { get; set; }
    public Currency Currency { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}