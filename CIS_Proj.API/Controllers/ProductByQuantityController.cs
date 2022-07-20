using Microsoft.AspNetCore.Mvc;
using Library.GUI_App.Models;
using CIS_Proj.API.Database;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CIS_Proj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductByQuantityController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public List<ProductByQuantity> Get()
        {
            return FakeProductDatabase.QuantityProducts;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
