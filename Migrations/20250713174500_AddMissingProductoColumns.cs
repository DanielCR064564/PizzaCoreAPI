using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaCoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingProductoColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns one by one with IF NOT EXISTS checking
            migrationBuilder.Sql(@"
                ALTER TABLE `Productos` 
                ADD COLUMN IF NOT EXISTS `FechaActualizacion` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
                ADD COLUMN IF NOT EXISTS `FechaCreacion` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
                ADD COLUMN IF NOT EXISTS `Stock` int NOT NULL DEFAULT 0,
                ADD COLUMN IF NOT EXISTS `ApiId` varchar(36) CHARACTER SET utf8mb4 NULL,
                ADD COLUMN IF NOT EXISTS `SincronizadoConAPI` tinyint(1) NOT NULL DEFAULT 0,
                ADD COLUMN IF NOT EXISTS `Imagen` varchar(255) CHARACTER SET utf8mb4 NULL,
                ADD COLUMN IF NOT EXISTS `IngredientesTexto` text CHARACTER SET utf8mb4 NULL,
                ADD COLUMN IF NOT EXISTS `TiempoPreparacion` int NOT NULL DEFAULT 15;
            ");

            // Add trigger for FechaActualizacion automatic update
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS `productos_update_timestamp`;
                CREATE TRIGGER `productos_update_timestamp`
                BEFORE UPDATE ON `Productos`
                FOR EACH ROW
                SET NEW.FechaActualizacion = CURRENT_TIMESTAMP(6);
            ");

            // Add indexes if they don't exist
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS `idx_producto_api` ON `Productos` (`ApiId`);
                CREATE INDEX IF NOT EXISTS `idx_producto_nombre` ON `Productos` (`Nombre`);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the trigger
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS `productos_update_timestamp`;");

            // Drop the indexes
            migrationBuilder.Sql("DROP INDEX IF EXISTS `idx_producto_api` ON `Productos`;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS `idx_producto_nombre` ON `Productos`;");

            // Drop the columns
            migrationBuilder.Sql(@"
                ALTER TABLE `Productos` 
                DROP COLUMN IF EXISTS `FechaActualizacion`,
                DROP COLUMN IF EXISTS `FechaCreacion`,
                DROP COLUMN IF EXISTS `Stock`,
                DROP COLUMN IF EXISTS `ApiId`,
                DROP COLUMN IF EXISTS `SincronizadoConAPI`,
                DROP COLUMN IF EXISTS `Imagen`,
                DROP COLUMN IF EXISTS `IngredientesTexto`,
                DROP COLUMN IF EXISTS `TiempoPreparacion`;
            ");
        }
    }
}
