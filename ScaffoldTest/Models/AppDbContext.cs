using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ScaffoldTest.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwUserByUsernameEmail> VwUserByUsernameEmails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=test;User Id=sa;Password=Rubikscube22;TrustServerCertificate=True;"
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27EF14CD97");

            entity.ToTable("Users", "mwss");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<VwUserByUsernameEmail>(entity =>
        {
            entity.HasNoKey().ToView("vw_UserByUsernameEmail", "mwss");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("ID");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
