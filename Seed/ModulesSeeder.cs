using System.Text.Json;
using GenericApi.Models;

namespace GenericApi.Seed
{
    public class ModuleType
    {
        public string ModuleName { get; set; } = null!;
        public string[]? Permissions { get; set; } = [];

        public ModuleType[]? SubModule { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ModulesSeeder(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public void Seed()
        {
            var modulesJson = File.ReadAllText("Seed/Modules.json");
            var modules = JsonSerializer.Deserialize<List<ModuleType>>(modulesJson) ?? [];

            foreach (var module in modules)
            {
                // For top-level modules, grandParentId is null
                InsertModuleWithSubmodules(module, null, null);
            }
            _context.SaveChanges();
        }

        private void InsertModuleWithSubmodules(
            ModuleType moduleType,
            int? parentId,
            int? grandParentId
        )
        {
            var existingModule = _context
                .Modules.Where(m => m.ModuleName == moduleType.ModuleName)
                .FirstOrDefault(m => m.ParentId == parentId);
            if (existingModule != null)
                return;

            var module = new Module
            {
                ModuleName = moduleType.ModuleName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                GrandParentId = grandParentId,
                ParentId = parentId,
                ModuleStatus = true,
            };

            var insertedModule = _context.Modules.Add(module);
            _context.SaveChanges(); // Save to get the insertedModule.Entity.Id

            // Insert permissions for this module if any
            if (moduleType.Permissions != null && moduleType.Permissions.Length > 0)
            {
                foreach (var permission in moduleType.Permissions)
                {
                    var modulePermission = new ModulePermission
                    {
                        ModuleId = insertedModule.Entity.Id,
                        PermissionName = permission,
                        PermissionStatus = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };
                    _context.ModulePermissions.Add(modulePermission);
                }
                _context.SaveChanges();
            }

            if (moduleType.SubModule != null && moduleType.SubModule.Length > 0)
            {
                foreach (var subModule in moduleType.SubModule)
                {
                    InsertModuleWithSubmodules(
                        subModule,
                        insertedModule.Entity.Id,
                        parentId // parentId is the main module's id for sub-submodules, null for direct submodules
                    );
                }
            }
        }
    }
}
