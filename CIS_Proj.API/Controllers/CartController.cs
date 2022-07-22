using CIS_Proj.API.Database;
using Library.GUI_App.Models;
using CIS_Proj.API.EC;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CIS_Proj.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        [HttpGet]
        public Dictionary<string, List<Product>> Get()
        {
            return FakeProductDatabase.Carts;
        }

        //[HttpGet("{name}")]
        //public List<Product> Get(string name)
        //{
        //    return FakeProductDatabase.Carts.FirstOrDefault(c => c.Key == name).Value;
        //}

        // POST api/<CartController>
        [HttpPost]
        public void Post([FromBody] string cartName)
        {
            new CartEC().Create(cartName);
        }

        //// PUT api/<CartController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<CartController>/5
        [HttpPost("Delete")]
        public void Delete([FromBody] string cartName)
        {
            new CartEC().Delete(cartName);
        }
    }
}
