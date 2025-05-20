using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfScaffoldDemo.Migrations
{
    /// <inheritdoc />
    public partial class SP_AddUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = File.ReadAllText("Scripts/sp/add-users.sql");
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    DROP PROCEDURE mwss.AddUser;
                "
            );
        }
    }
}
