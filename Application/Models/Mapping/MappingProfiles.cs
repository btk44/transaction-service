using AutoMapper;
using TransactionService.Domain.Types;

namespace TransactionService.Application.Models;

public class MappingProfiles: Profile {
    public MappingProfiles()
    {
        CreateMap<Account, AccountDto>(); // no dto -> db for now
        CreateMap<Category, CategoryDto>(); // no dto -> db for now
        CreateMap<Transaction, TransactionDto>();
        CreateMap<TransactionDto, Transaction>();
    }
}