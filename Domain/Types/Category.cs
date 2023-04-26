namespace TransactionService.Domain.Types;

public class Category: Base {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public Category ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; }
}