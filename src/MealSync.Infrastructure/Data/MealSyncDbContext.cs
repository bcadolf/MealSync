using MealSync.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MealSync.Infrastructure.Data
{
    public class MealSyncDbContext : IdentityDbContext<ApplicationUser>
    {
        public MealSyncDbContext(DbContextOptions<MealSyncDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes { get; set; } = default!;
        public DbSet<Ingredient> Ingredients { get; set; } = default!;
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = default!;
        public DbSet<MealPlan> MealPlans { get; set; } = default!;
        public DbSet<GroceryList> GroceryLists { get; set; } = default!;
        public DbSet<GroceryListItem> GroceryListItems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Composite Key for RecipeIngredient
            builder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            // Configure relationships if conventions are not enough
            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);
        }
    }
}
