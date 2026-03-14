using System;
using System.Threading.Tasks;
using MealSync.Core.Entities;

namespace MealSync.Core.Interfaces
{
    public interface IGroceryListService
    {
        Task<GroceryList> GenerateGroceryListAsync(DateTime start, DateTime end, string? userId = null);
        Task<GroceryList?> GetLatestGroceryListAsync(string? userId = null);
        Task ToggleItemCheckedAsync(int listItemId);
    }
}
