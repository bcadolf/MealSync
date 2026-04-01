using MealSync.Core.Entities;
using MealSync.Core.Interfaces;
using MealSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealSync.Infrastructure.Services
{
    public class GroceryListService : IGroceryListService
    {
        private readonly MealSyncDbContext _context;

        public GroceryListService(MealSyncDbContext context)
        {
            _context = context;
        }

        public async Task<GroceryList> GenerateGroceryListAsync(DateTime start, DateTime end, string? userId = null)
        {
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var mealPlansQuery = _context.MealPlans
                .Include(mp => mp.Recipe)
                .ThenInclude(r => r!.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Where(mp => mp.Date >= start && mp.Date <= end);

            if (!string.IsNullOrEmpty(userId))
            {
                mealPlansQuery = mealPlansQuery.Where(mp => mp.UserId == userId);
            }

            var mealPlans = await mealPlansQuery.ToListAsync();

            // Aggregate duplicate ingredients based on matching Name and Unit
            // We group by Name (case-insensitive) instead of ID, as creating a recipe currently creates new ingredients
            var aggregatedItems = new Dictionary<(string Name, string Unit), (int IngredientId, decimal Quantity)>();

            foreach (var plan in mealPlans)
            {
                if (plan.Recipe?.RecipeIngredients == null) continue;

                // Scale ingredients if servings feature is implemented, for now just 1:1
                foreach (var ri in plan.Recipe.RecipeIngredients)
                {
                    if (ri.Ingredient == null || string.IsNullOrWhiteSpace(ri.Ingredient.Name)) continue;

                    var nameKey = ri.Ingredient.Name.Trim().ToLowerInvariant();
                    var unitKey = (ri.Unit ?? string.Empty).Trim().ToLowerInvariant();
                    var key = (nameKey, unitKey);

                    if (aggregatedItems.ContainsKey(key))
                    {
                        var existing = aggregatedItems[key];
                        aggregatedItems[key] = (existing.IngredientId, existing.Quantity + ri.Quantity);
                    }
                    else
                    {
                        aggregatedItems[key] = (ri.IngredientId, ri.Quantity);
                    }
                }
            }

            var groceryList = new GroceryList
            {
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                IsCompleted = false
            };

            foreach (var kvp in aggregatedItems)
            {
                groceryList.Items.Add(new GroceryListItem
                {
                    IngredientId = kvp.Value.IngredientId,
                    Quantity = kvp.Value.Quantity,
                    Unit = kvp.Key.Unit,
                    IsChecked = false
                });
            }

            _context.GroceryLists.Add(groceryList);
            await _context.SaveChangesAsync();

            // Return with populated ingredient entities for the UI
            return await _context.GroceryLists
                .Include(gl => gl.Items)
                .ThenInclude(i => i.Ingredient)
                .FirstAsync(gl => gl.ListId == groceryList.ListId);
        }

        public async Task<GroceryList?> GetLatestGroceryListAsync(string? userId = null)
        {
            var query = _context.GroceryLists
                .Include(gl => gl.Items)
                .ThenInclude(i => i.Ingredient)
                .OrderByDescending(gl => gl.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(gl => gl.UserId == userId);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task ToggleItemCheckedAsync(int listItemId)
        {
            var item = await _context.GroceryListItems.FindAsync(listItemId);
            if (item != null)
            {
                item.IsChecked = !item.IsChecked;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<GroceryListItem>> AddGroceryIngredientAsync(List<GroceryListItem> filteredAddedItems, GroceryList groceryList, string? userId = null)
        {

            foreach (GroceryListItem item in filteredAddedItems)
            {
                groceryList.Items.Add(item); // no need to call context since it was called in grocerylist and grocerylistitems
            }

            await _context.SaveChangesAsync();

            return filteredAddedItems;
        }

        public async Task<GroceryListItem> UpdateGroceryIngredientAsync(GroceryListItem groceryListItem)
        {
            await _context.SaveChangesAsync();
            return groceryListItem;
        }
    }
}
