using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MealSync.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
        public ICollection<GroceryList> GroceryLists { get; set; } = new List<GroceryList>();
    }
}
