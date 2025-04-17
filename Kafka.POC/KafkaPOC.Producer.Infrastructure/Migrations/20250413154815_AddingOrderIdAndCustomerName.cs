using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KafkaPOC.Producer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingOrderIdAndCustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "OutboxEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "OutboxEvents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "OutboxEventLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "OutboxEventLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "OutboxEvents");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OutboxEvents");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "OutboxEventLogs");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OutboxEventLogs");
        }
    }
}
