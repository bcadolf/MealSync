using System;

namespace MealSync.Core.Entities
{
    public class MealPlan
    {
        public int MealPlanId { get; set; }
        public DateTime Date { get; set; }
        public string MealType { get; set; } = string.Empty;

        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
