using Codifico.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Codifico.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShippersController : Controller
    {
        private readonly string _connectionString;

        public ShippersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Recupera la informacion de los transportistas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shippers>>> GetOrders()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var resultShippers = new List<Shippers>();
                    await connection.OpenAsync();

                    var query = "SELECT * FROM StoreSample.Sales.Shippers";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var Shippers = new Shippers
                                {
                                    Shipperid = reader.GetInt32(0),
                                    Companyname = reader.GetString(1)
                                };
                                resultShippers.Add(Shippers);
                            }
                            return Ok(resultShippers);
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
