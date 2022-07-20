using CIS_Proj.API.Database;
using Library.GUI_App.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CIS_Proj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductByWeightController : ControllerBase
    {
        // GET: api/<ProductByWeightController>
        [HttpGet]
        public List<ProductByWeight> Get()
        {
            return FakeProductDatabase.WeightProducts;
        }

        // GET api/<ProductByWeightController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductByWeightController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProductByWeightController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductByWeightController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
