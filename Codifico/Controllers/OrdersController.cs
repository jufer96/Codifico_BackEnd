using Codifico.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Codifico.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly string _connectionString;

        public OrdersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Recupera la informacion de las ordenes.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Oders>>> GetOrders(int CustId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var resultOrders = new List<Oders>();
                    await connection.OpenAsync();

                    var query = "SELECT * FROM StoreSample.Sales.Orders where custid = " + CustId;
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var Order = new Oders
                                {
                                    OrderId = reader.GetInt32(0),
                                    RequiredDate = reader.GetDateTime(4),
                                    ShippedDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                    ShipName = reader.GetString(8),
                                    ShipAddress = reader.GetString(9),
                                    ShipCity = reader.GetString(10)
                                };
                                resultOrders.Add(Order);
                            }
                            return Ok(resultOrders);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Ingresa una nueva orden.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> NewOrder([FromBody] OrdersRequest ordersRequest)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand();

                            var query = "INSERT INTO StoreSample.Sales.Orders (Empid, Custid, Orderdate, Requireddate, ShippedDate, Shipperid, Freight, Shipname, ShipAddress, Shipcity, Shipregion, Shippostalcode, Shipcountry) " +
                                        "VALUES (@Empid, @Custid, @Orderdate, @Requireddate, @ShippedDate, @Shipperid, @Freight, @Shipname, @ShipAddress, @Shipcity, @Shipregion, @shippostalcode, @Shipcountry) " +
                                        "SELECT SCOPE_IDENTITY();";
                            using (command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Empid", ordersRequest.EmpId);
                                command.Parameters.AddWithValue("@Custid", ordersRequest.CustId);
                                command.Parameters.AddWithValue("@Orderdate", ordersRequest.OrderDate);
                                command.Parameters.AddWithValue("@Requireddate", ordersRequest.RequiredDate);
                                command.Parameters.AddWithValue("@ShippedDate", ordersRequest.ShippedDate);
                                command.Parameters.AddWithValue("@Shipperid", ordersRequest.ShipperId);
                                command.Parameters.AddWithValue("@Freight", ordersRequest.Freight);
                                command.Parameters.AddWithValue("@Shipname", ordersRequest.ShipName);
                                command.Parameters.AddWithValue("@ShipAddress", ordersRequest.ShipAddress);
                                command.Parameters.AddWithValue("@Shipcity", ordersRequest.ShipCity);
                                command.Parameters.AddWithValue("@Shipregion", DBNull.Value);
                                command.Parameters.AddWithValue("@shippostalcode", DBNull.Value);
                                command.Parameters.AddWithValue("@Shipcountry", ordersRequest.ShipCountry);

                                int OderId = Convert.ToInt32(await command.ExecuteScalarAsync());

                                if (OderId > 0)
                                {
                                    query = "INSERT INTO StoreSample.Sales.OrderDetails (Orderid, Productid, Unitprice, Qty, Discount) " +
                                            "VALUES (@Orderid, @Productid, @Unitprice, @Qty, @Discount) " +
                                            "SELECT SCOPE_IDENTITY();";
                                    using (command = new SqlCommand(query, connection, transaction))
                                    {
                                        command.Parameters.AddWithValue("@Orderid", OderId);
                                        command.Parameters.AddWithValue("@Productid", ordersRequest.OrderDetails.ProductId);
                                        command.Parameters.AddWithValue("@Unitprice", ordersRequest.OrderDetails.UnitPrice);
                                        command.Parameters.AddWithValue("@Qty", ordersRequest.OrderDetails.Qty);
                                        command.Parameters.AddWithValue("@Discount", ordersRequest.OrderDetails.Discount);

                                        var respOderDetails = await command.ExecuteScalarAsync();

                                        transaction.Commit();
                                        return Ok();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
                        }
                    }
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
