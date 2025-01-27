namespace Codifico.Models
{
    /// <summary>
    /// Informacion para una nueva orden
    /// </summary>
    public class OrdersRequest
    {
        public int EmpId { get; set; }
        public int CustId { get; set; }
        public int ShipperId { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal Freight { get; set; }
        public string ShipCountry { get; set; }
        public OrdersDetails? OrderDetails { get; set; }
    }
}
