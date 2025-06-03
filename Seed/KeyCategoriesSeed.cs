using System.Text.Json;
using GenericApi.Models;

namespace GenericApi.Seed
{
    public class KeyCategoriesSeed(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public void Seed()
        {
            var keyCategoriesJson = File.ReadAllText("Seed/KeyCategories.json");
            var keyCategories =
                JsonSerializer.Deserialize<List<KeyCategory>>(keyCategoriesJson) ?? [];

            foreach (var keyCategory in keyCategories)
            {
                if (!_context.KeyCategories.Any(kc => kc.CategoryName == keyCategory.CategoryName))
                {
                    keyCategory.CreatedAt = DateTime.UtcNow;
                    keyCategory.UpdatedAt = DateTime.UtcNow;
                    _context.KeyCategories.Add(keyCategory);
                }
            }
            _context.SaveChanges();
        }
    }
}
