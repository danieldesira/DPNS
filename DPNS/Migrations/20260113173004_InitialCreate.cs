using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DPNS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apps",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_name = table.Column<string>(type: "character varying", nullable: false),
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "time with time zone", nullable: false),
                    url = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("projects_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "push_notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    title = table.Column<string>(type: "character varying", nullable: false),
                    text = table.Column<string>(type: "character varying", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    app_url = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("push_notifications_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    email = table.Column<string>(type: "character varying", nullable: false),
                    password = table.Column<string>(type: "character varying", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "time with time zone", nullable: false),
                    last_login_at = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "push_subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    endpoint = table.Column<string>(type: "character varying", nullable: false),
                    auth = table.Column<string>(type: "character varying", nullable: false),
                    p256dh = table.Column<string>(type: "character varying", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    app_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("push_subscriptions_pk", x => x.id);
                    table.ForeignKey(
                        name: "push_subscriptions_apps_fk",
                        column: x => x.app_id,
                        principalTable: "apps",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_users_pk", x => x.id);
                    table.ForeignKey(
                        name: "app_users_apps_fk",
                        column: x => x.app_id,
                        principalTable: "apps",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "app_users_users_fk",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_verification_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    verification_code = table.Column<string>(type: "character varying", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_verification_tokens_pk", x => x.id);
                    table.ForeignKey(
                        name: "user_verification_tokens_users_fk",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_users_app_id",
                table: "app_users",
                column: "app_id");

            migrationBuilder.CreateIndex(
                name: "IX_app_users_user_id",
                table: "app_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "projects_unique",
                table: "apps",
                column: "app_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "projects_unique_uuid",
                table: "apps",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_push_subscriptions_app_id",
                table: "push_subscriptions",
                column: "app_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_verification_tokens_user_id",
                table: "user_verification_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "users_unique",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_users");

            migrationBuilder.DropTable(
                name: "push_notifications");

            migrationBuilder.DropTable(
                name: "push_subscriptions");

            migrationBuilder.DropTable(
                name: "user_verification_tokens");

            migrationBuilder.DropTable(
                name: "apps");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
