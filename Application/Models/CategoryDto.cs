namespace TransactionService.Application.Models;

public class CategoryDto {
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int ParentId { get; set; }
    public bool Active { get; set; }
    public List<CategoryDto> Subcategories { get; set; }
}