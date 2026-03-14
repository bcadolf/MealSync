using System.Collections.Generic;
using System.Threading.Tasks;
using MealSync.Core.Entities;

namespace MealSync.Core.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllRecipesAsync();
        Task<Recipe?> GetRecipeByIdAsync(int id);
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(int id);
    }
}
