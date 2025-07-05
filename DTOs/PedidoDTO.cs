using System;
using System.Collections.Generic;

namespace PizzaCoreAPI.DTOs
{
    public class PedidoDTO
    {
        public string Id { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; }
        public string ClienteId { get; set; } = string.Empty;
        public string? EmpleadoId { get; set; }
        public decimal Total { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
    }

    public class PedidoDetalleDTO
    {
        public string Id { get; set; } = string.Empty;
        public Guid ProductoId { get; set; }
        public string Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public ProductoDTO Producto { get; set; }
    }

    public class CrearPedidoDTO
    {
        public string ClienteId { get; set; } = string.Empty;
        public string? EmpleadoId { get; set; }
        public string Estado { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; }
    }
}
