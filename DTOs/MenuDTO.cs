using System.Collections.Generic;

namespace PizzaCoreAPI.DTOs
{
    public class MenuDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioTotal { get; set; }
        public bool Activo { get; set; }
        public List<ProductoDTO> Productos { get; set; }
    }

    public class CrearMenuDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public List<Guid> ProductosIds { get; set; }
    }
}
