namespace PizzaCoreAPI.Models
{
    // Estado for Pedidos
    public enum EstadoPedido
    {
        Pendiente = 0,    
        EnPreparacion = 1, 
        Listo = 2,     
        EnCamino = 3,  
        Entregado = 4,     
        Cancelado = 5     
    }

    // Estado for Facturas  
    public enum EstadoFactura
    {
        Pendiente = 0,     
        Pagado = 1,       
        Anulado = 2,       
        Vencido = 3        
    }

    // Estado for CuentasPorCobrar
    public enum EstadoCuentaPorCobrar
    {
        Pendiente = 0,   
        Pagado = 1,       
        Parcial = 2,       
        Vencido = 3,      
        Incobrable = 4     
    }
}