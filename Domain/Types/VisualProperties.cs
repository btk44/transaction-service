namespace TransactionService.Domain.Types;

public class VisualProperties {
    public int Id { get; set; }
    public string ObjectName { get; set; }
    public int ObjectId { get; set; }
    public string Color { get; set; } = "#00000";
    public string Icon { get; set; } = string.Empty;
} 