using PizzaCoreAPI.DTOs;
using PizzaCoreAPI.Models;

namespace PizzaCoreAPI.Services
{
    public interface IFacturaService
    {
        /// <summary>
        /// Gets all invoices with their complete details
        /// </summary>
        /// <returns>Collection of all invoices</returns>
        Task<IEnumerable<FacturaDTO>> GetAllFacturasAsync();

        /// <summary>
        /// Gets a specific invoice by its ID
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Invoice DTO or null if not found</returns>
        Task<FacturaDTO?> GetFacturaByIdAsync(int id);

        /// <summary>
        /// Gets an invoice associated with a specific order
        /// </summary>
        /// <param name="pedidoId">Order ID</param>
        /// <returns>Invoice DTO or null if not found</returns>
        Task<FacturaDTO?> GetFacturaByPedidoIdAsync(string pedidoId);

        /// <summary>
        /// Creates an invoice from a delivered order
        /// </summary>
        /// <param name="pedidoId">Order ID to create invoice from</param>
        /// <returns>Created invoice DTO</returns>
        /// <exception cref="InvalidOperationException">When order is not delivered or already has an invoice</exception>
        Task<FacturaDTO> CreateFacturaFromPedidoAsync(string pedidoId);

        /// <summary>
        /// Updates the status of an invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="estado">New invoice status</param>
        /// <returns>True if updated successfully, false if invoice not found</returns>
        /// <exception cref="InvalidOperationException">When state transition is not allowed</exception>
        Task<bool> UpdateEstadoFacturaAsync(int id, EstadoFactura estado);

        /// <summary>
        /// Generates a PDF document for the specified invoice
        /// </summary>
        /// <param name="facturaId">Invoice ID</param>
        /// <returns>PDF file as byte array</returns>
        /// <exception cref="InvalidOperationException">When invoice is not found</exception>
        Task<byte[]> GenerarFacturaPDFAsync(int facturaId);

        /// <summary>
        /// Generates the next sequential invoice number
        /// </summary>
        /// <returns>Formatted invoice number (e.g., "FAC-00000001")</returns>
        Task<string> GenerarNumeroFacturaAsync();

        /// <summary>
        /// Gets invoices filtered by status
        /// </summary>
        /// <param name="estado">Invoice status to filter by</param>
        /// <returns>Collection of invoices with the specified status</returns>
        Task<IEnumerable<FacturaDTO>> GetFacturasByEstadoAsync(EstadoFactura estado);

        /// <summary>
        /// Gets invoices within a date range
        /// </summary>
        /// <param name="fechaInicio">Start date (inclusive)</param>
        /// <param name="fechaFin">End date (inclusive)</param>
        /// <returns>Collection of invoices within the date range</returns>
        Task<IEnumerable<FacturaDTO>> GetFacturasByFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Gets invoices for a specific customer
        /// </summary>
        /// <param name="clienteId">Customer ID</param>
        /// <returns>Collection of customer invoices</returns>
        Task<IEnumerable<FacturaDTO>> GetFacturasByClienteAsync(string clienteId);

        /// <summary>
        /// Validates if an invoice can be created from an order
        /// </summary>
        /// <param name="pedidoId">Order ID</param>
        /// <returns>True if invoice can be created, false otherwise</returns>
        Task<bool> PuedeCrearFacturaAsync(string pedidoId);

        /// <summary>
        /// Gets invoice statistics for reporting
        /// </summary>
        /// <param name="fechaInicio">Start date for statistics</param>
        /// <param name="fechaFin">End date for statistics</param>
        /// <returns>Invoice statistics object</returns>
        Task<EstadisticasFacturaDTO> GetEstadisticasAsync(DateTime fechaInicio, DateTime fechaFin);
    }

    /// <summary>
    /// DTO for invoice statistics
    /// </summary>
    public class EstadisticasFacturaDTO
    {
        public int TotalFacturas { get; set; }
        public decimal TotalMonto { get; set; }
        public decimal TotalIVA { get; set; }
        public decimal PromedioFactura { get; set; }
        public int FacturasPendientes { get; set; }
        public int FacturasPagadas { get; set; }
        public int FacturasAnuladas { get; set; }
        public int FacturasVencidas { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
        public decimal MontoPromedioDiario { get; set; }
        public decimal TasaCobranza { get; set; } // Percentage of paid invoices
    }
}