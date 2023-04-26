using Microsoft.EntityFrameworkCore;
using TransactionService.Domain.Types;

namespace TransactionService.Infrastructure;

public class ApplicationDbContextInitialiser{
    private ApplicationDbContext _dbContext;

    public ApplicationDbContextInitialiser(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Migrate(){
        await _dbContext.Database.MigrateAsync();
        await InsertData();  // to do: remove it later?
    }

    private async Task InsertData(){
        if(!_dbContext.Currencies.Any()){
            Console.WriteLine("=== Inserting currency data ===");

            var currencies = new List<Currency>() {
                new Currency() { Description = "United States dollar", Code = "USD" },
                new Currency() { Description = "Euro", Code = "EUR" },
                new Currency() { Description = "Japanese yen", Code = "JPY" },
                new Currency() { Description = "Sterling", Code = "GBP" },
                new Currency() { Description = "Australian dollar", Code = "AUD" },
                new Currency() { Description = "Canadian dollar", Code = "CAD" },
                new Currency() { Description = "Swiss franc", Code = "CHF" },
                new Currency() { Description = "Renminbi", Code = "CNY" },
                new Currency() { Description = "Hong Kong dollar", Code = "HKD" },
                new Currency() { Description = "New Zealand dollar", Code = "NZD" },
                new Currency() { Description = " Swedish krona", Code = "SEK" },
                new Currency() { Description = "South Korean won", Code = "KRW" },
                new Currency() { Description = "Singapore dollar", Code = "SGD" },
                new Currency() { Description = "Norwegian krone", Code = "NOK" },
                new Currency() { Description = "Mexican peso", Code = "MXN" },
                new Currency() { Description = "Indian rupee", Code = "INR" },
                new Currency() { Description = "Russian ruble", Code = "RUB" },
                new Currency() { Description = "South African rand", Code = "ZAR" },
                new Currency() { Description = "Turkish lira", Code = "TRY" },
                new Currency() { Description = "Brazilian real", Code = "BRL" },
                new Currency() { Description = "New Taiwan dollar", Code = "TWD" },
                new Currency() { Description = "Danish krone", Code = "DKK" },
                new Currency() { Description = "Polish złoty", Code = "PLN" },
                new Currency() { Description = "Thai baht", Code = "THB" },
                new Currency() { Description = "Indonesian rupiah", Code = "IDR" },
                new Currency() { Description = "Hungarian forint", Code = "HUF" },
                new Currency() { Description = "Czech koruna", Code = "CZK" },
                new Currency() { Description = "Israeli new shekel", Code = "ILS" },
                new Currency() { Description = "Chilean peso", Code = "CLP" },
                new Currency() { Description = "Philippine peso", Code = "PHP" },
                new Currency() { Description = "UAE dirham", Code = "AED" },
                new Currency() { Description = "Colombian peso", Code = "COP" },
                new Currency() { Description = "Saudi riyal", Code = "SAR" },
                new Currency() { Description = "Malaysian ringgit", Code = "MYR" },
                new Currency() { Description = "Romanian leu", Code = "RON" }
            };

            _dbContext.Currencies.AddRange(currencies);

            await _dbContext.SaveChangesAsync();
            return;
        }

        Console.WriteLine("=== Currency data already inserted ===");
    }
}