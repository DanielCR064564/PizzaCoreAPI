using System.ComponentModel.DataAnnotations;

namespace PizzaCoreAPI.DTOs
{
    public class PedidoDTO
    {
        public string Id { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string ClienteId { get; set; } = string.Empty;
        public string? EmpleadoId { get; set; }
        public decimal Total { get; set; }
        public string? MetodoPago { get; set; }
        public string? Notas { get; set; }
        public string? NumeroOrden { get; set; }
        public string? TerminalId { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();

        // Additional computed properties
        public int TotalProductos => Detalles.Sum(d => d.Cantidad);
        public bool RequiereEntrega => !string.IsNullOrEmpty(EmpleadoId);
        public string EstadoDisplay => Estado switch
        {
            "Pendiente" => "Pendiente",
            "EnPreparacion" => "En Preparación",
            "Listo" => "Listo",
            "EnCamino" => "En Camino",
            "Entregado" => "Entregado",
            "Cancelado" => "Cancelado",
            _ => Estado
        };
    }

    public class PedidoDetalleDTO
    {
        public string Id { get; set; } = string.Empty;
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public ProductoDTO Producto { get; set; } = new();
    }

    public class CrearPedidoDTO
    {
        [Required(ErrorMessage = "El ID del cliente es requerido")]
        [StringLength(255, ErrorMessage = "El ID del cliente no puede exceder 255 caracteres")]
        public string ClienteId { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "El ID del empleado no puede exceder 255 caracteres")]
        public string? EmpleadoId { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [RegularExpression("^(Pendiente|EnPreparacion|Listo|EnCamino|Entregado|Cancelado)$",
            ErrorMessage = "Estado debe ser: Pendiente, EnPreparacion, Listo, EnCamino, Entregado, o Cancelado")]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
        public string? MetodoPago { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }

        public DateTime? FechaPedido { get; set; }

        public decimal? Total { get; set; }

        [StringLength(100, ErrorMessage = "El número de orden no puede exceder 100 caracteres")]
        public string? NumeroOrden { get; set; }

        [StringLength(50, ErrorMessage = "El ID del terminal no puede exceder 50 caracteres")]
        public string? TerminalId { get; set; }

        [Required(ErrorMessage = "Los detalles del pedido son requeridos")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto en el pedido")]
        [MaxLength(50, ErrorMessage = "No se pueden agregar más de 50 productos por pedido")]
        public List<CrearPedidoDetalleDTO> Detalles { get; set; } = new();

        // Custom validation method
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles?.Any() == true)
            {
                for (int i = 0; i < Detalles.Count; i++)
                {
                    var detalle = Detalles[i];
                    if (detalle.Cantidad <= 0)
                    {
                        yield return new ValidationResult(
                            $"La cantidad del producto {i + 1} debe ser mayor a 0",
                            new[] { $"Detalles[{i}].Cantidad" });
                    }

                    if (detalle.Cantidad > 100)
                    {
                        yield return new ValidationResult(
                            $"La cantidad del producto {i + 1} no puede exceder 100 unidades",
                            new[] { $"Detalles[{i}].Cantidad" });
                    }

                    if (detalle.ProductoId == Guid.Empty)
                    {
                        yield return new ValidationResult(
                            $"El ID del producto {i + 1} es requerido",
                            new[] { $"Detalles[{i}].ProductoId" });
                    }
                }

                // Check for duplicate products
                var duplicateProducts = Detalles
                    .GroupBy(d => d.ProductoId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                foreach (var duplicateProduct in duplicateProducts)
                {
                    yield return new ValidationResult(
                        $"El producto {duplicateProduct} está duplicado. Combine las cantidades en una sola línea.",
                        new[] { "Detalles" });
                }
            }
        }
    }

    public class CrearPedidoDetalleDTO
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public Guid ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
        public int Cantidad { get; set; } = 1;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0")]
        public decimal? PrecioUnitario { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal? Subtotal { get; set; }
    }

    public class ActualizarEstadoPedidoDTO
    {
        [Required(ErrorMessage = "El estado es requerido")]
        [RegularExpression("^(Pendiente|EnPreparacion|Listo|EnCamino|Entregado|Cancelado)$",
            ErrorMessage = "Estado debe ser: Pendiente, EnPreparacion, Listo, EnCamino, Entregado, o Cancelado")]
        public string Estado { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La observación no puede exceder 200 caracteres")]
        public string? Observacion { get; set; }

        public DateTime? FechaEstimadaEntrega { get; set; }
    }

    public class ActualizarPedidoDTO
    {
        [StringLength(255, ErrorMessage = "El ID del empleado no puede exceder 255 caracteres")]
        public string? EmpleadoId { get; set; }

        [RegularExpression("^(Pendiente|EnPreparacion|Listo|EnCamino|Entregado|Cancelado)$",
            ErrorMessage = "Estado debe ser: Pendiente, EnPreparacion, Listo, EnCamino, Entregado, o Cancelado")]
        public string? Estado { get; set; }

        [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
        public string? MetodoPago { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }

        public DateTime? FechaEntrega { get; set; }
    }

    // DTO for order summary/statistics
    public class ResumenPedidoDTO
    {
        public string Id { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int TotalProductos { get; set; }
        public string? NombreEmpleado { get; set; }
    }

    // DTO for order search/filtering
    public class FiltrarPedidosDTO
    {
        public string? ClienteId { get; set; }
        public string? EmpleadoId { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal? MontoMinimo { get; set; }
        public decimal? MontoMaximo { get; set; }
        public int Pagina { get; set; } = 1;
        public int ElementosPorPagina { get; set; } = 10;
    }
}