using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Redit_api.Models;
using Redit_api.Models.Status;

#nullable disable

namespace Redit_api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:post_status", "active,archived")
                .Annotation("Npgsql:Enum:post_status.post_status", "active,archived")
                .Annotation("Npgsql:Enum:user_role", "super_user,user")
                .Annotation("Npgsql:Enum:user_role.user_role", "user,super_user")
                .Annotation("Npgsql:Enum:user_status", "do_not_disturb,idle,invisible,offline,online")
                .Annotation("Npgsql:Enum:user_status.user_status", "offline,online,do_not_disturb,idle,invisible");

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false),
                    embeds = table.Column<string[]>(type: "text[]", nullable: false),
                    commenter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aura = table.Column<int>(type: "integer", nullable: false),
                    post_id = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "community",
                schema: "public",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    profile_picture = table.Column<string>(type: "text", nullable: true),
                    owner_username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    pinned_post_ids = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "post",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    aura = table.Column<int>(type: "integer", nullable: false),
                    original_poster = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    community = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    embeds = table.Column<string[]>(type: "text[]", nullable: false),
                    status = table.Column<PostStatus>(type: "post_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    aura = table.Column<int>(type: "integer", nullable: false),
                    bio = table.Column<string>(type: "text", nullable: true),
                    profile_picture = table.Column<string>(type: "text", nullable: true),
                    account_status = table.Column<UserStatus>(type: "user_status", nullable: false),
                    role = table.Column<UserRole>(type: "user_role", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "community",
                schema: "public");

            migrationBuilder.DropTable(
                name: "post",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user",
                schema: "public");
        }
    }
}
