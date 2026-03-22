using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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

        public async Task<List<Recipe>> GetAllRecipesAsync(string? userId = null)
        {
            var query = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(r => r.UserId == userId || r.IsPublic);
            }

            return await query.ToListAsync();
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
            var existingRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeId == recipe.RecipeId);

            if (existingRecipe != null)
            {
                // Update scalar properties
                _context.Entry(existingRecipe).CurrentValues.SetValues(recipe);

                // Make a copy of the incoming ingredients BEFORE modifying tracked collections
                var newIngredients = recipe.RecipeIngredients
                    ?.Select(ri => new RecipeIngredient
                    {
                        RecipeId = recipe.RecipeId,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit,
                        Ingredient = new Ingredient
                        {
                            Name = ri.Ingredient?.Name ?? string.Empty
                        }
                    })
                    .ToList();

                // Remove old ingredients by replacing them
                _context.RecipeIngredients.RemoveRange(existingRecipe.RecipeIngredients);

                if (newIngredients != null)
                {
                    foreach (var ri in newIngredients)
                    {
                        _context.RecipeIngredients.Add(ri);
                    }
                }

                await _context.SaveChangesAsync();
            }
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
