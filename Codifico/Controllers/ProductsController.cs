using Codifico.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Codifico.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly string _connectionString;

        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Recupera la informacion de los productos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetOrders()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var resultProducts = new List<Products>();
                    await connection.OpenAsync();

                    var query = "SELECT * FROM StoreSample.Production.Products";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var Products = new Products
                                {
                                    Productid = reader.GetInt32(0),
                                    Productname = reader.GetString(1)
                                };
                                resultProducts.Add(Products);
                            }
                            return Ok(resultProducts);
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
