namespace TransactionService.Domain.Types;

public class Transaction: Base {
    public int OwnerId { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public int CategoryId { get; set; }
    public string GroupKey { get; set; }
    public string Payee { get; set; }
    public string Comment { get; set; }
    public Account Account { get; set; }
    public Category Category { get; set; }
}