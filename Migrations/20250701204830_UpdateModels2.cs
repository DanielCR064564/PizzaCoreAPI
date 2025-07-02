using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaCoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Pizzas_PizzaId",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_EmpleadoId1",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_EmpleadoId1",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EmpleadoId1",
                table: "Pedidos");

            migrationBuilder.AlterColumn<string>(
                name: "EmpleadoId",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_EmpleadoId",
                table: "Pedidos",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Pizzas_PizzaId",
                table: "PedidoDetalles",
                column: "PizzaId",
                principalTable: "Pizzas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_AspNetUsers_EmpleadoId",
                table: "Pedidos",
                column: "EmpleadoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Pizzas_PizzaId",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_EmpleadoId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_EmpleadoId",
                table: "Pedidos");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "Pedidos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "EmpleadoId1",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_EmpleadoId1",
                table: "Pedidos",
                column: "EmpleadoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Pizzas_PizzaId",
                table: "PedidoDetalles",
                column: "PizzaId",
                principalTable: "Pizzas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_AspNetUsers_EmpleadoId1",
                table: "Pedidos",
                column: "EmpleadoId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
