using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MealSync.Core.Entities;
using MealSync.Core.Interfaces;
using MealSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Reflection.Metadata.Ecma335;

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
                query = query.Where(r => r.UserId == userId);
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
            var normIngredients = new List<RecipeIngredient>();

            foreach (var ri in recipe.RecipeIngredients) {
                var name = ri.Ingredient?.Name?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var savedIngredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
            
            
            Ingredient ingredientToUse;

            if (savedIngredient != null)
            {
                ingredientToUse = savedIngredient;
            } else
            {
                ingredientToUse = new Ingredient { Name = name };
                _context.Ingredients.Add(ingredientToUse);
            }

            normIngredients.Add(new RecipeIngredient
            {
                Ingredient = ingredientToUse,
                Quantity = ri.Quantity,
                Unit = ri.Unit
            });
            }

            recipe.RecipeIngredients = normIngredients;

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

        if (existingRecipe == null)
            return;

        // Update scalar fields
        _context.Entry(existingRecipe).CurrentValues.SetValues(recipe);

        // Remove old ingredients
        _context.RecipeIngredients.RemoveRange(existingRecipe.RecipeIngredients);

        var normalizedIngredients = new List<RecipeIngredient>();

        foreach (var ri in recipe.RecipeIngredients)
        {
            var name = ri.Ingredient?.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                continue;

            var existingIngredient = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());

            Ingredient ingredientToUse;

            if (existingIngredient != null)
            {
                ingredientToUse = existingIngredient;
            }
            else
            {
                ingredientToUse = new Ingredient { Name = name };
                _context.Ingredients.Add(ingredientToUse);
            }

            normalizedIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.RecipeId,
                Ingredient = ingredientToUse,
                Quantity = ri.Quantity,
                Unit = ri.Unit
            });
        }

        existingRecipe.RecipeIngredients = normalizedIngredients;

        await _context.SaveChangesAsync();
    }

    public async Task<List<Recipe>> GetPublicRecipesAsync()
        {
            return await _context.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Where(r => r.IsPublic)
            .ToListAsync();
        }

    public async Task<List<Recipe>> SearchPublicRecipesAsync(string? searchTerm)
        {
            var query = _context.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Where(r => r.IsPublic)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var trimmedSearch = searchTerm.Trim().ToLower();
                query = query.Where(r => r.Title.ToLower().Contains(trimmedSearch));
            }

            return await query.ToListAsync();
        }

    public async Task<bool> CopyPublicRecipeToUserAsync(int recipeId, string userId)
        {
            var sourceRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.IsPublic);

            if (sourceRecipe == null || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var alreadyExists = await _context.Recipes
                .AnyAsync(r => r.UserId == userId && r.Title.ToLower() == sourceRecipe.Title.ToLower());

            if (alreadyExists)
            {
                return false;
            }

            var copiedRecipe = new Recipe
            {
                Title = sourceRecipe.Title,
                Description = sourceRecipe.Description,
                Instructions = sourceRecipe.Instructions,
                PrepTime = sourceRecipe.PrepTime,
                CookTime = sourceRecipe.CookTime,
                Servings = sourceRecipe.Servings,
                IsPublic = false,
                UserId = userId,
                RecipeIngredients = new List<RecipeIngredient>()
            };

            foreach (var ri in sourceRecipe.RecipeIngredients)
            {
                Ingredient ingredientToUse;

                var ingredientName = ri.Ingredient?.Name?.Trim();

                if(string.IsNullOrWhiteSpace(ingredientName))
                {
                    continue;
                }

                var existingIngredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Name.ToLower() == ingredientName.ToLower());

                if (existingIngredient != null)
                {
                    ingredientToUse = existingIngredient;
                }
                else
                {
                    ingredientToUse = new Ingredient
                    {
                        Name = ingredientName
                    };

                    _context.Ingredients.Add(ingredientToUse);
                }

                copiedRecipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    Ingredient = ingredientToUse,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                });
            }
            _context.Recipes.Add(copiedRecipe);
            await _context.SaveChangesAsync();

            return true;
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
