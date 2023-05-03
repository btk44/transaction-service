using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Models;

namespace TransactionService.Application.Commands;

public class TransactionSearchCommand {
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public double AmountFrom { get; set; } 
    public double AmountTo { get; set; }
    public string Payee{ get; set; }
    public List<int> Categories { get; set; }
    public string Comment { get; set; }
    public bool Active { get; set; }
    public bool ActiveDefined { get; set; }
    public List<int> Accounts { get; set; }
    public bool IncludeGroupTransactions { get; set; }
    public int Take { get; set; }
    public int Offset { get; set; }
}

public class TransactionSearchCommandHandler
{
    private IApplicationDbContext _dbContext;
    private IMapper _transactionMapper;

    public TransactionSearchCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _transactionMapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(TransactionSearchCommand command)
    {
        var transactionQuery = _dbContext.Transactions
                .Where(x => x.OwnerId == command.OwnerId && 
                            x.Amount >= command.AmountFrom && 
                            x.Amount <= command.AmountTo && 
                            x.Date  >= command.DateFrom &&
                            x.Date <= command.DateTo);

        if( command.Id > 0)
            transactionQuery = transactionQuery.Where(x => x.Id == command.Id);

        if(command.ActiveDefined)
            transactionQuery = transactionQuery.Where(x => x.Active == command.Active);

        if(!string.IsNullOrEmpty(command.Payee)){
            var payee = command.Payee.ToLower();
            transactionQuery = transactionQuery.Where(x => x.Payee.ToLower().Contains(payee));
        }

        if(!string.IsNullOrEmpty(command.Comment)){
            var comment = command.Comment.ToLower();
            transactionQuery = transactionQuery.Where(x => x.Comment.ToLower().Contains(comment));
        }

        if(command.Categories.Any())
            transactionQuery = transactionQuery.Where(x => command.Categories.Contains(x.CategoryId));

        if(command.Accounts.Any())
            transactionQuery = transactionQuery.Where(x => command.Accounts.Contains(x.AccountId));

        var transactions = await transactionQuery
                                    .Skip(command.Offset)
                                    .Take(command.Take)
                                    .Select(x => _transactionMapper.Map<TransactionDto>(x)).ToListAsync();

        if(command.IncludeGroupTransactions){
            var transactionsWithGroup = transactions.Where(x => !string.IsNullOrEmpty(x.GroupKey));
            var groupKeys = transactionsWithGroup.Select(x => x.GroupKey);

            var connectedTransactions = await _dbContext.Transactions
                                .Where(x => x.OwnerId == command.OwnerId && groupKeys.Contains(x.GroupKey)).ToListAsync();

            foreach(var transaction in transactionsWithGroup){
                transaction.GroupTransactions = connectedTransactions
                                    .Where(x => x.GroupKey == transaction.GroupKey && x.Id != transaction.Id)
                                    .Select(x => _transactionMapper.Map<TransactionDto>(x)).ToList();
            }
        }

        return transactions;
    }
}