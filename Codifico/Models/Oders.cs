namespace Codifico.Models
{
    /// <summary>
    /// Informacion de las ordenes
    /// </summary>
    public class Oders
    {
        public int OrderId { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }

    }
}
