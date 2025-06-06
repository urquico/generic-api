﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GenericApi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<KeyCategory> KeyCategories { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModulePermission> ModulePermissions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleModulePermission> RoleModulePermissions { get; set; }

    public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSecurityQuestion> UserSecurityQuestions { get; set; }

    public virtual DbSet<UserSpecialPermission> UserSpecialPermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        =>
        optionsBuilder.UseSqlServer(
            "Server=URQUICO\\SQLEXPRESS;Database=simplevia_common;Trusted_Connection=True;TrustServerCertificate=True;"
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__key_cate__3213E83F82551AA5");

            entity.ToTable("key_categories", "mwss");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName).HasMaxLength(255).HasColumnName("category_name");
            entity.Property(e => e.CategoryValue).HasMaxLength(255).HasColumnName("category_value");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__modules__3213E83F0DE0AD96");

            entity.ToTable("modules", "fmis");

            entity.HasIndex(e => e.GrandParentId, "IX_modules_grand_parent_id");

            entity.HasIndex(e => e.ParentId, "IX_modules_parent_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.GrandParentId).HasColumnName("grand_parent_id");
            entity.Property(e => e.ModuleName).HasMaxLength(255).HasColumnName("module_name");
            entity.Property(e => e.ModuleStatus).HasColumnName("module_status");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity
                .HasOne(d => d.GrandParent)
                .WithMany(p => p.InverseGrandParent)
                .HasForeignKey(d => d.GrandParentId)
                .HasConstraintName("FK_Modules_GrandParent");

            entity
                .HasOne(d => d.Parent)
                .WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Modules_Parent");
        });

        modelBuilder.Entity<ModulePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__module_p__3213E83F39BC6C4C");

            entity.ToTable("module_permissions", "fmis");

            entity.HasIndex(e => e.ModuleId, "IX_module_permissions_module_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity
                .Property(e => e.PermissionName)
                .HasMaxLength(255)
                .HasColumnName("permission_name");
            entity.Property(e => e.PermissionStatus).HasColumnName("permission_status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity
                .HasOne(d => d.Module)
                .WithMany(p => p.ModulePermissions)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Permissions_Modules");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__refresh___3213E83FB6BA19AF");

            entity.ToTable("refresh_tokens", "fmis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasMaxLength(100).HasColumnName("created_by");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IpAddress).HasMaxLength(50).HasColumnName("ip_address");
            entity.Property(e => e.IsRevoked).HasDefaultValue(false).HasColumnName("is_revoked");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.RevokedBy).HasMaxLength(100).HasColumnName("revoked_by");
            entity.Property(e => e.Token).HasMaxLength(500).HasColumnName("token");
            entity.Property(e => e.UserAgent).HasMaxLength(255).HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83FF091C0D4");

            entity.ToTable("roles", "fmis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.RoleName).HasMaxLength(100).HasColumnName("role_name");
            entity.Property(e => e.RoleStatus).HasColumnName("role_status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<RoleModulePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role_per__3213E83FC80CA33C");

            entity.ToTable("role_module_permissions", "fmis");

            entity.HasIndex(e => e.PermissionId, "IX_role_permissions_permission_id");

            entity.HasIndex(e => e.RoleId, "IX_role_permissions_role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity
                .HasOne(d => d.Permission)
                .WithMany(p => p.RoleModulePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_RolePermissions_Permissions");

            entity
                .HasOne(d => d.Role)
                .WithMany(p => p.RoleModulePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermissions_Roles");
        });

        modelBuilder.Entity<SecurityQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__security__3213E83F71F37A23");

            entity.ToTable("security_questions", "fmis");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.QuestionStatus).HasColumnName("question_status");
            entity.Property(e => e.QuestionText).HasMaxLength(500).HasColumnName("question_text");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F79457AED");

            entity.ToTable("users", "fmis");

            entity.HasIndex(e => e.StatusId, "IX_users_status_id");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164B08C60EB").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
            entity.Property(e => e.FirstName).HasMaxLength(100).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasMaxLength(100).HasColumnName("last_name");
            entity.Property(e => e.MiddleName).HasMaxLength(100).HasColumnName("middle_name");
            entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("password");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity
                .HasOne(d => d.Status)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Users_StatusKeyCategory");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_rol__3213E83F4106E08B");

            entity.ToTable("user_roles", "fmis");

            entity.HasIndex(e => e.RoleId, "IX_user_roles_role_id");

            entity.HasIndex(e => e.UserId, "IX_user_roles_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity
                .HasOne(d => d.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_user_roles_role");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_user_roles_user");
        });

        modelBuilder.Entity<UserSecurityQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_sec__3213E83FDAD4F491");

            entity.ToTable("user_security_questions", "fmis");

            entity.HasIndex(
                e => e.SecurityQuestionId,
                "IX_user_security_questions_security_question_id"
            );

            entity.HasIndex(e => e.UserId, "IX_user_security_questions_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity
                .Property(e => e.SecurityAnswer)
                .HasMaxLength(255)
                .HasColumnName("security_answer");
            entity.Property(e => e.SecurityQuestionId).HasColumnName("security_question_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity
                .HasOne(d => d.SecurityQuestion)
                .WithMany(p => p.UserSecurityQuestions)
                .HasForeignKey(d => d.SecurityQuestionId)
                .HasConstraintName("FK_UserSecurityQuestions_SecurityQuestions");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.UserSecurityQuestions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserSecurityQuestions_User");
        });

        modelBuilder.Entity<UserSpecialPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_spe__3213E83FFBD906A5");

            entity.ToTable("user_special_permissions", "fmis");

            entity.HasIndex(e => e.UserId, "IX_user_special_permissions_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessStatus).HasColumnName("access_status");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.UserSpecialPermissions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserSpecialPermissions_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
