using Microsoft.AspNetCore.Mvc;
using Library.GUI_App.Models;
using CIS_Proj.API.Database;
using CIS_Proj.API.EC;
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
            return new ProductByQuantityEC().Get();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost("Create")]
        public ProductByQuantity Create(ProductByQuantity productByQuantity)
        {
            if (productByQuantity.Id == -1)
            {
                productByQuantity.Id = FakeProductDatabase.NextId();
                FakeProductDatabase.QuantityProducts.Add(productByQuantity);
                return productByQuantity;
            }

            var productToUpdate = FakeProductDatabase.QuantityProducts.FirstOrDefault(p => p.Id == productByQuantity.Id);
            if (productToUpdate != null)
            {
                FakeProductDatabase.QuantityProducts.Remove(productToUpdate);
                FakeProductDatabase.QuantityProducts.Add(productByQuantity);
            }
            return productByQuantity;
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
