namespace Codifico.Models
{
    /// <summary>
    /// Informacion de los detalles para una nueva orden
    /// </summary>
    public class OrdersDetails
    {
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
    }
}
