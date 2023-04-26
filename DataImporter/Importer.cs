using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Types;

namespace TransactionService.DataImporter;

public class TransactionRow{
    [CsvHelper.Configuration.Attributes.Index(0)]
    public DateTime Date { get; set; }

    [CsvHelper.Configuration.Attributes.Index(1)]
    public int AccountId { get; set; }

    [CsvHelper.Configuration.Attributes.Index(2)]
    public string AccountName { get; set; }

    [CsvHelper.Configuration.Attributes.Index(3)]
    public double Amount { get; set; }

    [CsvHelper.Configuration.Attributes.Index(4)]
    public string Payee { get; set; }

    [CsvHelper.Configuration.Attributes.Index(5)]
    public string CategoryId { get; set; }

    [CsvHelper.Configuration.Attributes.Index(6)]
    public string CategoryName { get; set; }

    [CsvHelper.Configuration.Attributes.Index(7)]
    public string SubCategoryId { get; set; }

    [CsvHelper.Configuration.Attributes.Index(8)]
    public string SubCategoryName { get; set; }

    [CsvHelper.Configuration.Attributes.Index(9)]
    public string Comment { get; set; }
}

public class Importer {
    private IApplicationDbContext _dbContext;

    public Importer(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportFromCsv(){
        IEnumerable<TransactionRow> records;

        using (var reader = new StreamReader("DataImporter\\TestData.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<TransactionRow>().ToList();
        }

        var accounts = await _dbContext.Accounts.ToDictionaryAsync(x => x.Name);
        var categories = await _dbContext.Categories.ToDictionaryAsync(x => x.Name);
        var currency = await _dbContext.Currencies.FirstOrDefaultAsync(x => x.Code == "PLN");

        foreach(TransactionRow record in records){
            if (record.AccountId <= 0)
                throw new Exception("no account");

            if (!accounts.ContainsKey(record.AccountName)){
                var newAccount = new Account() {
                    Name = record.AccountName,
                    Amount = 0,
                    Currency = currency, // change this manually later
                    OwnerId = 1
                };

                await _dbContext.Accounts.AddAsync(newAccount);
                accounts.Add(newAccount.Name, newAccount);
            }

            if (string.IsNullOrEmpty(record.CategoryId))
                throw new Exception("no category");
            
            if (string.IsNullOrEmpty(record.SubCategoryId))
                throw new Exception("no subcategory");

            if(!categories.ContainsKey(record.CategoryName)){
                var newCategory = new Category(){
                    Name = record.CategoryName,
                    OwnerId = 1,
                    SubCategories = new List<Category>() 
                };

                if (!string.IsNullOrEmpty(record.SubCategoryName)){
                    newCategory.SubCategories.Add(new Category(){
                            Name = record.SubCategoryName,
                            OwnerId = 1
                        });
                }

                await _dbContext.Categories.AddAsync(newCategory);
                categories.Add(newCategory.Name, newCategory);
            } else if(!string.IsNullOrEmpty(record.SubCategoryName) && categories[record.CategoryName].SubCategories.All(x => x.Name != record.SubCategoryName)) {
                var parentCategory = categories[record.CategoryName];

                parentCategory.SubCategories.Add(new Category(){
                    Name = record.SubCategoryName,
                    OwnerId = 1
                });
            }


            await _dbContext.Transactions.AddAsync(new Transaction(){
                OwnerId = 1,
                Date = record.Date,
                Account = accounts[record.AccountName],
                Amount = record.Amount,
                Payee = record.Payee,
                Category = categories[record.CategoryName],
                Comment = record.Comment
            });
        }

        await _dbContext.SaveChangesAsync();

        var visualproperties = await _dbContext.VisualProperties.ToListAsync();

        foreach(var account in accounts){
            var accountEntity = account.Value;
            accountEntity.Amount = await _dbContext.Transactions.Where(x => x.AccountId == accountEntity.Id).SumAsync(x => x.Amount);
            if (!visualproperties.Any(x => x.ObjectId == accountEntity.Id && x.ObjectName == "Account")){
                await _dbContext.VisualProperties.AddAsync(new VisualProperties(){
                    ObjectId = accountEntity.Id,
                    ObjectName = nameof(accountEntity)
                });
            }
        }

        foreach(var category in categories){
            var categoryEntity = category.Value;
            if (!visualproperties.Any(x => x.ObjectId == categoryEntity.Id && x.ObjectName == "Category")){
                await _dbContext.VisualProperties.AddAsync(new VisualProperties(){
                    ObjectId = categoryEntity.Id,
                    ObjectName = nameof(categoryEntity)
                });
            } 
            foreach(var subcategory in categoryEntity.SubCategories){
                if (!visualproperties.Any(x => x.ObjectId == subcategory.Id && x.ObjectName == "Category")){
                    await _dbContext.VisualProperties.AddAsync(new VisualProperties(){
                        ObjectId = subcategory.Id,
                        ObjectName = nameof(subcategory)
                    });
                } 
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}