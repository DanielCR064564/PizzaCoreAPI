using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaCoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductosMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiId",
                table: "Productos",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Productos",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Productos",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<string>(
                name: "Imagen",
                table: "Productos",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IngredientesTexto",
                table: "Productos",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "SincronizadoConAPI",
                table: "Productos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TiempoPreparacion",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 15);

            migrationBuilder.CreateIndex(
                name: "idx_producto_api",
                table: "Productos",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "idx_producto_nombre",
                table: "Productos",
                column: "Nombre");

            // Add trigger for FechaActualizacion automatic update
            migrationBuilder.Sql(@"
                CREATE TRIGGER productos_update_timestamp
                BEFORE UPDATE ON Productos
                FOR EACH ROW
                SET NEW.FechaActualizacion = CURRENT_TIMESTAMP(6);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_producto_api",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "idx_producto_nombre",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ApiId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "IngredientesTexto",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "SincronizadoConAPI",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TiempoPreparacion",
                table: "Productos");
        }
    }
}
