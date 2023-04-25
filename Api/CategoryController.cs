using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.Models;
using TransactionService.Application.Commands;
using AutoMapper;
using TransactionService.Application.Interfaces;

namespace TransactionService.Api;

[ApiController]
[Route("api/[controller]")]
public class CategoryController
{
    private IApplicationDbContext _dbContext;
    private IMapper _mapper;

    public CategoryController(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<CategoryDto>>> Search([FromBody] CategorySearchCommand command){
        var categorySearchCommandHandler = new CategorySearchCommandHandler(_dbContext, _mapper);
        return await categorySearchCommandHandler.Handle(command);
    }
}