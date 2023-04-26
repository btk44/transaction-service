using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Common;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Models;
using TransactionService.Domain.Types;

namespace TransactionService.Application.Commands;

public class TransactionProcessCommand {
    public int ProcessingUserId { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}

public class TransactionProcessCommandHandler {
    private IApplicationDbContext _dbContext;
    private IMapper _transactionMapper;

    public TransactionProcessCommandHandler(IApplicationDbContext dbContext, IMapper transactionMapper){
        _dbContext = dbContext;
        _transactionMapper = transactionMapper;
    }

    public async Task<Either<List<TransactionDto>, AppException>> Handle(TransactionProcessCommand command){
        if(command.Transactions.Any(x => x.OwnerId <= 0)){
            var incorrectTransactionIds = string.Join(", ", command.Transactions.Where(x => x.OwnerId <= 0).Select(x => x.Id));
            return new AppException($"Incorrect owner id in transactions: { incorrectTransactionIds }");
        }

        var accountIdList = command.Transactions.Select(x => x.AccountId).Distinct();
        var categoryIdList = command.Transactions.Select(x => x.CategoryId).Distinct();
        var transactionIdList = command.Transactions.Where(x => x.Id > 0).Select(x => x.Id);

        var accounts = await _dbContext.Accounts
                                    .Where(x => accountIdList.Contains(x.Id))
                                    .ToDictionaryAsync(x => x.Id);

        var categories = await _dbContext.Categories
                                    .Where(x => categoryIdList.Contains(x.Id))
                                    .ToDictionaryAsync(x => x.Id);

        var existingTransactions = await _dbContext.Transactions
                                    .Where(x => transactionIdList.Contains(x.Id))
                                    .ToListAsync();

        var existingTransactionIds = existingTransactions.Select(x => x.Id);
        var missingTransactions = transactionIdList.Except(existingTransactionIds).ToList();

        if(missingTransactions.Any()){
            return new AppException($"Missing transactions: { string.Join(", ", missingTransactions) }");
        }

        var processedEntities = new List<Transaction>();

        foreach(var commandTransaction in command.Transactions){
            Account account;
            Category category;

            if(!accounts.TryGetValue(commandTransaction.AccountId, out account)){
                return new AppException($"Account does not exist for transaction: {commandTransaction.Id}");
            }

            if(!categories.TryGetValue(commandTransaction.CategoryId, out category)){
                return new AppException($"Category does not exist for transaction: {commandTransaction.Id}");
            }

            if(commandTransaction.OwnerId != account.OwnerId){
                return new AppException($"Owner id between account and transaction does not match for transaction: {commandTransaction.Id}");
            }

            if(commandTransaction.OwnerId != category.OwnerId){
                return new AppException($"Owner id between category and transaction does not match for transaction: {commandTransaction.Id}");
            }

            Transaction transactionEntity = existingTransactions.FirstOrDefault(x => x.Id == commandTransaction.Id);
            if (transactionEntity == null){
                transactionEntity = new Transaction();
                _dbContext.Transactions.Add(transactionEntity);  // will that work?
            } 
            
            _transactionMapper.Map(commandTransaction, transactionEntity);
            transactionEntity.Account = account;
            transactionEntity.Category = category;

            processedEntities.Add(transactionEntity);
        }

        if(await _dbContext.SaveChangesAsync() <= 0){
            return new AppException("Save error - please try again");
        }

        return processedEntities.Select(x => _transactionMapper.Map<TransactionDto>(x)).ToList();  
    }
}