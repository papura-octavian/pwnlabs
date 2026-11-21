using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pwnlabs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomToLabInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "LabInstances",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LabInstances_RoomId",
                table: "LabInstances",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabInstances_Rooms_RoomId",
                table: "LabInstances",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabInstances_Rooms_RoomId",
                table: "LabInstances");

            migrationBuilder.DropIndex(
                name: "IX_LabInstances_RoomId",
                table: "LabInstances");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "LabInstances");
        }
    }
}
