using System.Collections.Generic;

namespace MealSync.Core.Entities
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}
