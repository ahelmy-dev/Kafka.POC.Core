using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KafkaPOC.Consumer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingOrderIdAndCustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Orders");
        }
    }
}
