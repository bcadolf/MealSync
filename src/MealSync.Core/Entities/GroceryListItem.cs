namespace MealSync.Core.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class GroceryListItem
    {
        [Key]
        public int ListItemId { get; set; }

        public int ListId { get; set; }
        public GroceryList? GroceryList { get; set; }

        public int IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
    }
}
