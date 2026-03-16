using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealSync.Core.Entities;
using MealSync.Core.Interfaces;
using MealSync.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealSync.Infrastructure.Services
{
    public class MealPlanService : IMealPlanService
    {
        private readonly MealSyncDbContext _context;

        public MealPlanService(MealSyncDbContext context)
        {
            _context = context;
        }

        public async Task<List<MealPlan>> GetMealPlansAsync(DateTime start, DateTime end, string? userId = null)
        {
            var query = _context.MealPlans
                .Include(mp => mp.Recipe)
                .Where(mp => mp.Date >= start && mp.Date <= end);

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(mp => mp.UserId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task<MealPlan> AddRecipeToPlanAsync(MealPlan mealPlan)
        {
            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync();
            return mealPlan;
        }

        public async Task RemoveRecipeFromPlanAsync(int mealPlanId)
        {
            var mealPlan = await _context.MealPlans.FindAsync(mealPlanId);
            if (mealPlan != null)
            {
                _context.MealPlans.Remove(mealPlan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
