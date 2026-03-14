using System.Collections.Generic;
using System.Threading.Tasks;
using MealSync.Core.Entities;
using MealSync.Core.Interfaces;
using MealSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealSync.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly MealSyncDbContext _context;

        public RecipeService(MealSyncDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeId == id);
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            _context.Entry(recipe).State = EntityState.Modified;

            // Handle ingredients updates (basic implementation for now)
            // A more robust implementation would diff the lists.

            await _context.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}
