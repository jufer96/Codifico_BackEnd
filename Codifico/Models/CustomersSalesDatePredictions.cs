using System;

namespace Codifico.Models
{
    /// <summary>
    /// Informacion de las fechas de la proxima orden por cliente
    /// </summary>
    public class CustomersSalesDatePredictions
    {
        public int CustId { get; set; }
        public string CustomerName { get; set; }
        public DateTime LastOrderDate { get; set; }
        public DateTime NextPredictedOrder { get; set; }

    }
}
