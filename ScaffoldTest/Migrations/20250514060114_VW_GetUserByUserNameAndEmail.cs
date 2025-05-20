using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfScaffoldDemo.Migrations
{
    /// <inheritdoc />
    public partial class VW_GetUserByUserNameAndEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = File.ReadAllText("Scripts/vw/get-user-by-username-and-email.sql");
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    DROP VIEW IF EXISTS mwss.vw_GetUserByUsernameAndEmail;
                "
            );
        }
    }
}
