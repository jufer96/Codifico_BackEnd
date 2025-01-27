using Codifico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Codifico.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly string _connectionString;

        public CustomersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Recupera la informacion de las fechas de la proxima orden por cliente.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomersSalesDatePredictions>>> GetOrders()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var resultDatePredictions = new List<CustomersSalesDatePredictions>();
                    await connection.OpenAsync();

                    var query = "WITH OrderDays AS ( " +
                        "SELECT custid, " +
                        "DATEDIFF(DAY, OrderDate, LEAD(OrderDate) OVER (PARTITION BY custid ORDER BY OrderDate)) AS DaysBetweenOrders " +
                        "FROM [StoreSample].[Sales].[Orders]), " +
                        "AverageOrderDays AS ( " +
                        "SELECT custid, " +
                        "AVG(DaysBetweenOrders) AS AvgDaysBetweenOrders " +
                        "FROM OrderDays WHERE DaysBetweenOrders IS NOT NULL " +
                        "GROUP BY custid) " +
                        "SELECT " +
                        "c.custid, " +
                        "c.contactname, " +
                        "MAX(o.OrderDate) AS LastOrderDate, " +
                        "DATEADD(DAY, a.AvgDaysBetweenOrders, MAX(o.OrderDate)) AS NextPredictedOrder " +
                        "FROM [StoreSample].[Sales].[Customers] c " +
                        "JOIN [StoreSample].[Sales].[Orders] o ON c.custid = o.custid " +
                        "JOIN AverageOrderDays a ON c.custid = a.custid " +
                        "GROUP BY c.custid, c.contactname, a.AvgDaysBetweenOrders";

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var DatePredictions = new CustomersSalesDatePredictions
                                {
                                    CustId = reader.GetInt32(0),
                                    CustomerName = reader.GetString(1),
                                    LastOrderDate = reader.GetDateTime(2),
                                    NextPredictedOrder = reader.GetDateTime(3)
                                    
                                };
                                resultDatePredictions.Add(DatePredictions);
                            }
                            return Ok(resultDatePredictions);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
            }

        }
    }
}
