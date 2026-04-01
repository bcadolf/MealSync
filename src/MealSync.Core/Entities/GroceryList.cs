using System;
using System.Collections.Generic;

namespace MealSync.Core.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class GroceryList
    {
        [Key]
        public int ListId { get; set; }
        public DateTime CreatedAt { get => _date;
            set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        private DateTime _date;
        public bool IsCompleted { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<GroceryListItem> Items { get; set; } = new List<GroceryListItem>();
    }
}
