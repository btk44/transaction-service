using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Models;
using TransactionService.Application.Commands;
using AutoMapper;
using TransactionService.Application.Interfaces;

namespace TransactionService.Api;

[ApiController]
[Route("api/[controller]")]
public class TransactionController
{
    private IApplicationDbContext _dbContext;
    private IMapper _mapper;

    public TransactionController(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<TransactionDto>>> Search([FromBody] TransactionSearchCommand command){
        var transactionSearchCommandHandler = new TransactionSearchCommandHandler(_dbContext, _mapper);
        return await transactionSearchCommandHandler.Handle(command);
    }

    [HttpPost("process")]
    public async Task<ActionResult<List<TransactionDto>>> Process([FromBody] TransactionProcessCommand command){
        var transactionProcessCommandHandler = new TransactionProcessCommandHandler(_dbContext, _mapper);
        return (await transactionProcessCommandHandler.Handle(command)).Match<ActionResult>(
            transactions => new OkObjectResult(transactions),
            exception => new BadRequestObjectResult(exception.Message)
        );
    }
}