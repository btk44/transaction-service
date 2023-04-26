using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Types;

namespace TransactionService.Infrastructure;
public class ApplicationDbContext : DbContext, IApplicationDbContext{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options){ }
    public DbSet<Account> Accounts { get; set;} 
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<VisualProperties> VisualProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { 
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>().ToTable("Account"); 
        modelBuilder.Entity<Currency>().ToTable("Currency"); 
        modelBuilder.Entity<Category>().ToTable("Category"); 
        modelBuilder.Entity<Transaction>().ToTable("Transaction"); 
        modelBuilder.Entity<VisualProperties>().ToTable("VisualProperties"); 

        Build(modelBuilder.Entity<Account>());
        Build(modelBuilder.Entity<Currency>());
        Build(modelBuilder.Entity<Category>());
        Build(modelBuilder.Entity<Transaction>());

        modelBuilder.Entity<Account>().HasMany(x => x.Transactions).WithOne(x => x.Account);        
        modelBuilder.Entity<Account>().HasOne(x => x.Currency);
        modelBuilder.Entity<Transaction>().HasOne(x => x.Category);
        modelBuilder.Entity<Category>().HasMany(x => x.SubCategories).WithOne(x => x.ParentCategory).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Category>().HasOne(x=> x.ParentCategory).WithMany(x=> x.SubCategories).HasForeignKey(x=> x.ParentId)
            .IsRequired(false).OnDelete(DeleteBehavior.Restrict);
    }    

    private void Build<T>(EntityTypeBuilder<T> entity) where T : Base
    {
        entity.Property(e => e.Id).UseIdentityColumn();
        entity.Property(e => e.Active).IsRequired().HasDefaultValue(true);
    }
}

