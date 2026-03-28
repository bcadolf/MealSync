using MealSync.Core.Entities;
using MealSync.Core.Interfaces;
using MealSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealSync.Infrastructure.Services
{
    public class IngredientService : IIngredientService
{
    private readonly MealSyncDbContext _context;

    public IngredientService(MealSyncDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ingredient>> SearchAsync(string query)
    {
        return await _context.Ingredients
            .Where(i => i.Name.Contains(query))
            .OrderBy(i => i.Name)
            .Take(20)
            .ToListAsync();
    }

    public async Task<Ingredient> AddAsync(string name)
    {
        var ingredient = new Ingredient { Name = name };
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }
}

}