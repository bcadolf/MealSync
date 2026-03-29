using MealSync.Core.Entities;

namespace MealSync.Core.Interfaces
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> SearchAsync(string query);
        Task<Ingredient> AddAsync(string name);
        Task<Ingredient?> GetByExactNameAsync(string name);
    }
}