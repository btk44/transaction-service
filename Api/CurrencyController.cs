using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Models;
using TransactionService.Application.Commands;
using TransactionService.Application.Interfaces;

namespace TransactionService.Api;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController
{
    private IApplicationDbContext _dbContext;

    public CurrencyController(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext; 
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<CurrencyDto>>> Search([FromBody] CurrencySearchCommand command){
        var currencySearchCommandHandler = new CurrencySearchCommandHandler(_dbContext);
        return await currencySearchCommandHandler.Handle(command);
    }
}