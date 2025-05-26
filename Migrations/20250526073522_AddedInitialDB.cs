using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenericApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedInitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "mwss");

            migrationBuilder.EnsureSchema(name: "fmis");

            migrationBuilder.CreateTable(
                name: "key_categories",
                schema: "mwss",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    category_value = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: true
                    ),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__key_cate__3213E83F82551AA5", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "modules",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    module_name = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    grand_parent_id = table.Column<int>(type: "int", nullable: true),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                    module_status = table.Column<bool>(type: "bit", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__modules__3213E83F0DE0AD96", x => x.id);
                    table.ForeignKey(
                        name: "FK_Modules_GrandParent",
                        column: x => x.grand_parent_id,
                        principalSchema: "fmis",
                        principalTable: "modules",
                        principalColumn: "id"
                    );
                    table.ForeignKey(
                        name: "FK_Modules_Parent",
                        column: x => x.parent_id,
                        principalSchema: "fmis",
                        principalTable: "modules",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    role_status = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__roles__3213E83FF091C0D4", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "security_questions",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_text = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    question_status = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__security__3213E83F71F37A23", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    middle_name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    last_name = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    email = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    password = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    status_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83F79457AED", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_StatusKeyCategory",
                        column: x => x.status_id,
                        principalSchema: "mwss",
                        principalTable: "key_categories",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "module_permissions",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    permission_name = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    permission_status = table.Column<int>(type: "int", nullable: true),
                    module_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__module_p__3213E83F39BC6C4C", x => x.id);
                    table.ForeignKey(
                        name: "FK_Permissions_Modules",
                        column: x => x.module_id,
                        principalSchema: "fmis",
                        principalTable: "modules",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_rol__3213E83F4106E08B", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_roles_role",
                        column: x => x.role_id,
                        principalSchema: "fmis",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_user_roles_user",
                        column: x => x.user_id,
                        principalSchema: "fmis",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_security_questions",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    security_question_id = table.Column<int>(type: "int", nullable: false),
                    security_answer = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_sec__3213E83FDAD4F491", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestions_SecurityQuestions",
                        column: x => x.security_question_id,
                        principalSchema: "fmis",
                        principalTable: "security_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestions_User",
                        column: x => x.user_id,
                        principalSchema: "fmis",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_special_permissions",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false),
                    access_status = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_spe__3213E83FFBD906A5", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserSpecialPermissions_User",
                        column: x => x.user_id,
                        principalSchema: "fmis",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "role_module_permissions",
                schema: "fmis",
                columns: table => new
                {
                    id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true,
                        defaultValueSql: "(getdate())"
                    ),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_by = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__role_per__3213E83FC80CA33C", x => x.id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions",
                        column: x => x.permission_id,
                        principalSchema: "fmis",
                        principalTable: "module_permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles",
                        column: x => x.role_id,
                        principalSchema: "fmis",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_module_permissions_module_id",
                schema: "fmis",
                table: "module_permissions",
                column: "module_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_modules_grand_parent_id",
                schema: "fmis",
                table: "modules",
                column: "grand_parent_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_modules_parent_id",
                schema: "fmis",
                table: "modules",
                column: "parent_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                schema: "fmis",
                table: "role_module_permissions",
                column: "permission_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_role_id",
                schema: "fmis",
                table: "role_module_permissions",
                column: "role_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                schema: "fmis",
                table: "user_roles",
                column: "role_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                schema: "fmis",
                table: "user_roles",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_security_questions_security_question_id",
                schema: "fmis",
                table: "user_security_questions",
                column: "security_question_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_security_questions_user_id",
                schema: "fmis",
                table: "user_security_questions",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_user_special_permissions_user_id",
                schema: "fmis",
                table: "user_special_permissions",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_users_status_id",
                schema: "fmis",
                table: "users",
                column: "status_id"
            );

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164B08C60EB",
                schema: "fmis",
                table: "users",
                column: "email",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "role_module_permissions", schema: "fmis");

            migrationBuilder.DropTable(name: "user_roles", schema: "fmis");

            migrationBuilder.DropTable(name: "user_security_questions", schema: "fmis");

            migrationBuilder.DropTable(name: "user_special_permissions", schema: "fmis");

            migrationBuilder.DropTable(name: "module_permissions", schema: "fmis");

            migrationBuilder.DropTable(name: "roles", schema: "fmis");

            migrationBuilder.DropTable(name: "security_questions", schema: "fmis");

            migrationBuilder.DropTable(name: "users", schema: "fmis");

            migrationBuilder.DropTable(name: "modules", schema: "fmis");

            migrationBuilder.DropTable(name: "key_categories", schema: "mwss");
        }
    }
}
