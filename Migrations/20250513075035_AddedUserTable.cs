using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfScaffoldDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "mwss");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "mwss",
                columns: table => new
                {
                    ID = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Email = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: true
                    ),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC27EF14CD97", x => x.ID);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Users", schema: "mwss");
        }
    }
}
