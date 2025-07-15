namespace PizzaCoreAPI.DTOs
{
    public class SyncFacturaDTO
    {
        public int IdFactura { get; set; }
        public DateTime FechaFactura { get; set; }
        public int FkCliente { get; set; }
        public string TipoEntrega { get; set; } = "Local";
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public int? EmpleadoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Notas { get; set; }

        // Optional: Include terminal info for tracking sync source
        public string? TerminalId { get; set; }
    }

    public class SyncFacturaBatchDTO
    {
        public List<SyncFacturaDTO> Facturas { get; set; } = new List<SyncFacturaDTO>();
        public string TerminalId { get; set; } = string.Empty;
        public DateTime SyncTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class SyncFacturaResultDTO
    {
        public int TotalReceived { get; set; }
        public int Synced { get; set; }
        public int Updated { get; set; }
        public int Failed { get; set; }
        public List<SyncFacturaErrorDTO> Errors { get; set; } = new List<SyncFacturaErrorDTO>();
    }

    public class SyncFacturaErrorDTO
    {
        public int FacturaId { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}