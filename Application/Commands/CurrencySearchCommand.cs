using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Models;

namespace TransactionService.Application.Commands;

public class CurrencySearchCommand {
    public string Code { get; set; }
    public int Id { get; set; }
    public string Description { get; set; }
}

public class CurrencySearchCommandHandler
{
    private IApplicationDbContext _dbContext;

    public CurrencySearchCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CurrencyDto>> Handle(CurrencySearchCommand command)
    {
        var currencyQuery = _dbContext.Currencies.Where(x => true);

        if(!string.IsNullOrEmpty(command.Code)){
            var code = command.Code.ToLower();
            currencyQuery = currencyQuery.Where(x => x.Code.ToLower().Contains(code));
        }

        if(command.Id > 0)
            currencyQuery = currencyQuery.Where(x => x.Id == command.Id);

        if(!string.IsNullOrEmpty(command.Description)){
            var description = command.Description.ToLower();
            currencyQuery = currencyQuery.Where(x => x.Description.ToLower().Contains(description));
        }

        return await currencyQuery.Select(x => new CurrencyDto() {
            Id = x.Id, Code = x.Code, Description = x.Description
        }).ToListAsync();
    }
}