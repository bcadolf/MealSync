using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealSync.Core.Entities;

namespace MealSync.Core.Interfaces
{
    public interface IMealPlanService
    {
        Task<List<MealPlan>> GetMealPlansAsync(DateTime start, DateTime end, string? userId = null);
        Task<MealPlan> AddRecipeToPlanAsync(MealPlan mealPlan);
        Task RemoveRecipeFromPlanAsync(int mealPlanId);
    }
}
