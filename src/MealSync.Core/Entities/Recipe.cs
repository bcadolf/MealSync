using System.Collections.Generic;

namespace MealSync.Core.Entities
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public bool IsPublic { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
