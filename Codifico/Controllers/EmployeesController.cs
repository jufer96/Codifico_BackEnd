using Codifico.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Codifico.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : Controller
    {
        private readonly string _connectionString;

        public EmployeesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Recupera la informacion de los empleados.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var resultEmployees = new List<Employees>();
                    await connection.OpenAsync();

                    var query = "SELECT * FROM StoreSample.HR.Employees";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var Employees = new Employees
                                {
                                    Empid = reader.GetInt32(0),
                                    FullName = reader.GetString(2) + " " + reader.GetString(1)
                                };
                                resultEmployees.Add(Employees);
                            }
                            return Ok(resultEmployees);
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
