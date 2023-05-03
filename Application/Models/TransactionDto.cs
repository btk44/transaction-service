namespace TransactionService.Application.Models;

public class TransactionDto {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int AccountId { get; set; }
    public double Amount { get; set; }
    public string Payee { get; set; }
    public int CategoryId { get; set; }
    public string Comment { get; set; }
    public string GroupKey { get; set; }
    public List<TransactionDto> GroupTransactions { get; set; }
    public bool Active { get; set; }
}