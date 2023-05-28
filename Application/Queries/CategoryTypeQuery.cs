using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Models;

namespace TransactionService.Application.Commands;

public class CategoryTypeQueryHandler {
    private IApplicationDbContext _dbContext;
    private IMapper _categoryMapper;

    public CategoryTypeQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryMapper = mapper;
    }

    public async Task<List<CategoryTypeDto>> Handle()
    {
        return await _dbContext.CategoryTypes.Select(x => _categoryMapper.Map<CategoryTypeDto>(x)).ToListAsync();
    }
}