using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Models;

namespace TransactionService.Application.Commands;

public class CategorySearchCommand {
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public int ParentId { get; set; }
    public int TypeId { get; set; }
    public bool Active { get; set; }
    public bool ActiveDefined { get; set; }
    public bool ReturnTreeStructure { get; set; }
}

public class CategorySearchCommandHandler
{
    private IApplicationDbContext _dbContext;
    private IMapper _categoryMapper;

    public CategorySearchCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _categoryMapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(CategorySearchCommand command)
    {
        var categoryQuery = _dbContext.Categories
                                .Where(x => x.OwnerId == command.OwnerId);

        if(command.ActiveDefined)
            categoryQuery = categoryQuery.Where(x => x.Active == command.Active);

        if(!string.IsNullOrEmpty(command.Name)){
            var categoryName = command.Name.ToLower();
            categoryQuery = categoryQuery.Where(x => x.Name.ToLower().Contains(categoryName));
        }

        if(command.Id > 0)
            categoryQuery = categoryQuery.Where(x => x.Id == command.Id);

        if(command.ParentId > 0)
            categoryQuery = categoryQuery.Where(x => x.ParentId == command.ParentId);

        if(command.TypeId > 0)
            categoryQuery = categoryQuery.Where(x => x.TypeId == command.TypeId);

        var categories = await categoryQuery.Select(x => _categoryMapper.Map<CategoryDto>(x)).ToListAsync();

        // consider movig it elswhere:
        var categoriesIds = categories.Select(x => x.Id);
        var visualProps = await _dbContext.VisualProperties.Where(x => x.ObjectName == "Category" && categoriesIds.Contains(x.ObjectId)).ToDictionaryAsync(x => x.ObjectId);
        categories.ForEach(x => x.Color = visualProps[x.Id].Color);
        //----

        if(!command.ReturnTreeStructure){
            categories.ForEach(x =>  x.Subcategories = null);
            return categories;
        }

        var categoriesTree = categories.Where(x => x.ParentId == 0).ToList();
        Dictionary<int, CategoryDto> parentCategories = categories.Where(x => x.ParentId == 0).ToDictionary(x => x.Id);
        foreach(var subcategory in categories.Where(x => x.ParentId > 0)){
            var parentId = subcategory.ParentId;
            if(parentCategories.ContainsKey(subcategory.ParentId)){
                if (parentCategories[parentId].Subcategories == null)
                    parentCategories[parentId].Subcategories = new List<CategoryDto>();

                parentCategories[parentId].Subcategories.Add(subcategory);
                continue;
            }

            categoriesTree.Add(subcategory);
        }
    
        return categoriesTree;
    }
}