using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaCoreAPI.Migrations
{
    public partial class UpdateGuidToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Primero eliminamos las restricciones de clave foránea
                ALTER TABLE [PedidoDetalles] DROP CONSTRAINT [FK_PedidoDetalles_Pedidos_PedidoId];
                ALTER TABLE [Pedidos] DROP CONSTRAINT [FK_Pedidos_Usuarios_ClienteId];
                ALTER TABLE [Pedidos] DROP CONSTRAINT [FK_Pedidos_Usuarios_EmpleadoId];

                -- Eliminamos la tabla temporal para los detalles
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PedidoDetalles_Temp]') AND type in (N'U'))
                    DROP TABLE [PedidoDetalles_Temp];

                -- Creamos una tabla temporal para los detalles
                SELECT Id, PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal, FacturaId
                INTO [PedidoDetalles_Temp]
                FROM [PedidoDetalles];

                -- Eliminamos las tablas existentes
                DROP TABLE [PedidoDetalles];
                DROP TABLE [Pedidos];

                -- Recreamos la tabla Pedidos con el nuevo tipo de Id
                CREATE TABLE [Pedidos] (
                    Id nvarchar(450) NOT NULL,
                    ClienteId nvarchar(450) NOT NULL,
                    EmpleadoId nvarchar(450) NULL,
                    FechaPedido datetime2 NOT NULL,
                    FechaEntrega datetime2 NULL,
                    Estado nvarchar(max) NULL,
                    Total decimal(18,2) NOT NULL,
                    MetodoPago nvarchar(max) NULL,
                    Notas nvarchar(max) NULL,
                    CONSTRAINT PK_Pedidos PRIMARY KEY (Id)
                );

                -- Recreamos la tabla PedidoDetalles con el nuevo tipo de Id
                CREATE TABLE [PedidoDetalles] (
                    Id nvarchar(450) NOT NULL,
                    PedidoId nvarchar(450) NOT NULL,
                    ProductoId uniqueidentifier NOT NULL,
                    Cantidad int NOT NULL,
                    PrecioUnitario decimal(18,2) NOT NULL,
                    Subtotal decimal(18,2) NOT NULL,
                    FacturaId uniqueidentifier NULL,
                    CONSTRAINT PK_PedidoDetalles PRIMARY KEY (Id),
                    CONSTRAINT FK_PedidoDetalles_Pedidos_PedidoId FOREIGN KEY (PedidoId) REFERENCES [Pedidos] (Id)
                );

                -- Insertamos los datos desde la tabla temporal
                INSERT INTO [PedidoDetalles] (Id, PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal, FacturaId)
                SELECT Id, PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal, FacturaId
                FROM [PedidoDetalles_Temp];

                -- Eliminamos la tabla temporal
                DROP TABLE [PedidoDetalles_Temp];

                -- Recreamos las restricciones de clave foránea
                ALTER TABLE [Pedidos] ADD CONSTRAINT [FK_Pedidos_Usuarios_ClienteId] FOREIGN KEY (ClienteId) REFERENCES [AspNetUsers] (Id);
                ALTER TABLE [Pedidos] ADD CONSTRAINT [FK_Pedidos_Usuarios_EmpleadoId] FOREIGN KEY (EmpleadoId) REFERENCES [AspNetUsers] (Id);
                ALTER TABLE [PedidoDetalles] ADD CONSTRAINT [FK_PedidoDetalles_Pedidos_PedidoId] FOREIGN KEY (PedidoId) REFERENCES [Pedidos] (Id);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Primero eliminamos las restricciones de clave foránea
                ALTER TABLE [PedidoDetalles] DROP CONSTRAINT [FK_PedidoDetalles_Pedidos_PedidoId];
                ALTER TABLE [Pedidos] DROP CONSTRAINT [FK_Pedidos_Usuarios_ClienteId];
                ALTER TABLE [Pedidos] DROP CONSTRAINT [FK_Pedidos_Usuarios_EmpleadoId];

                -- Eliminamos las tablas existentes
                DROP TABLE [PedidoDetalles];
                DROP TABLE [Pedidos];

                -- Recreamos la tabla Pedidos con el tipo de Id original
                CREATE TABLE [Pedidos] (
                    Id int IDENTITY(1,1) NOT NULL,
                    ClienteId nvarchar(450) NOT NULL,
                    EmpleadoId nvarchar(450) NULL,
                    FechaPedido datetime2 NOT NULL,
                    FechaEntrega datetime2 NULL,
                    Estado nvarchar(max) NULL,
                    Total decimal(18,2) NOT NULL,
                    MetodoPago nvarchar(max) NULL,
                    Notas nvarchar(max) NULL,
                    CONSTRAINT PK_Pedidos PRIMARY KEY (Id)
                );

                -- Recreamos la tabla PedidoDetalles con el tipo de Id original
                CREATE TABLE [PedidoDetalles] (
                    Id int IDENTITY(1,1) NOT NULL,
                    PedidoId int NOT NULL,
                    ProductoId uniqueidentifier NOT NULL,
                    Cantidad int NOT NULL,
                    PrecioUnitario decimal(18,2) NOT NULL,
                    Subtotal decimal(18,2) NOT NULL,
                    FacturaId uniqueidentifier NULL,
                    CONSTRAINT PK_PedidoDetalles PRIMARY KEY (Id),
                    CONSTRAINT FK_PedidoDetalles_Pedidos_PedidoId FOREIGN KEY (PedidoId) REFERENCES [Pedidos] (Id)
                );

                -- Recreamos las restricciones de clave foránea
                ALTER TABLE [Pedidos] ADD CONSTRAINT [FK_Pedidos_Usuarios_ClienteId] FOREIGN KEY (ClienteId) REFERENCES [AspNetUsers] (Id);
                ALTER TABLE [Pedidos] ADD CONSTRAINT [FK_Pedidos_Usuarios_EmpleadoId] FOREIGN KEY (EmpleadoId) REFERENCES [AspNetUsers] (Id);
                ALTER TABLE [PedidoDetalles] ADD CONSTRAINT [FK_PedidoDetalles_Pedidos_PedidoId] FOREIGN KEY (PedidoId) REFERENCES [Pedidos] (Id);
            ");
        }
    }
}
