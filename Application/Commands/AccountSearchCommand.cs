using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Models;

namespace TransactionService.Application.Commands;

public class AccountSearchCommand {
    public int OwnerId { get; set; }
    public int[] Currencies { get; set; }
    public string Name { get; set; }
    public double AmountFrom { get; set; } 
    public double AmountTo { get; set; }
    public int Id { get; set; }
    public bool Active { get; set; }
    public bool ActiveDefined { get; set; }
}

public class AccountSearchCommandHandler
{
    private IApplicationDbContext _dbContext;
    private IMapper _mapper;

    public AccountSearchCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<AccountDto>> Handle(AccountSearchCommand command)
    {
        var accountQuery = _dbContext.Accounts
                .Where(x => x.OwnerId == command.OwnerId &&
                            x.Amount >= command.AmountFrom &&
                            x.Amount <= command.AmountTo);

        if (command.ActiveDefined)
            accountQuery = accountQuery.Where(x => command.Active == x.Active);

        if (command.Currencies.Any())
            accountQuery = accountQuery.Where(x => command.Currencies.Contains(x.CurrencyId));

        if(!string.IsNullOrEmpty(command.Name)){
            var name = command.Name.ToLower();
            accountQuery = accountQuery.Where(x => x.Name.ToLower().Contains(name));
        }

        if(command.Id > 0)
            accountQuery = accountQuery.Where(x => x.Id == command.Id);

        return await accountQuery.Select(x => _mapper.Map<AccountDto>(x)).ToListAsync();
    }
}