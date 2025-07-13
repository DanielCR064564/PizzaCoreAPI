using System;

namespace PizzaCoreAPI.DTOs
{
    public class MetodoDePagoDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class CrearMetodoDePagoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
