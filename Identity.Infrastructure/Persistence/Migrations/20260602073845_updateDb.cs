using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAtUtc",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LockReason",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedUntilUtc",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiresAtUtc",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetTokenHash",
                table: "users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "sessions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAtUtc",
                table: "sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "sessions",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorLogin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TargetType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TargetId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TargetName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_PasswordResetTokenHash",
                table: "users",
                column: "PasswordResetTokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_Action",
                table: "audit_log",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_ActorLogin",
                table: "audit_log",
                column: "ActorLogin");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_ActorUserId",
                table: "audit_log",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_TimestampUtc",
                table: "audit_log",
                column: "TimestampUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropIndex(
                name: "IX_users_PasswordResetTokenHash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "DeactivatedAtUtc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LockReason",
                table: "users");

            migrationBuilder.DropColumn(
                name: "LockedUntilUtc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiresAtUtc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenHash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "sessions");

            migrationBuilder.DropColumn(
                name: "LastUsedAtUtc",
                table: "sessions");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "sessions");
        }
    }
}
