using TransactionService.Domain.Types;
using Microsoft.EntityFrameworkCore;

namespace TransactionService.Application.Interfaces;

public interface IApplicationDbContext{
    public DbSet<Account> Accounts { get; } 
    public DbSet<Transaction> Transactions { get; }
    public DbSet<Category> Categories { get; }
    public DbSet<CategoryType> CategoryTypes { get; }
    public DbSet<Currency> Currencies { get; }
    DbSet<VisualProperties> VisualProperties { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}