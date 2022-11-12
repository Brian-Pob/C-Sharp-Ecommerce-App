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
        [HttpGet("Carts/{name}")]
        public List<ProductByQuantity> Get(string name)
        {
            return new ProductByQuantityEC().GetCart(name);
        }

        // POST api/<ValuesController>
        // Adds to inventory
        [HttpPost("AddOrUpdate")]
        public ProductByQuantity AddOrUpdate(ProductByQuantity productByQuantity)
        {
            return new ProductByQuantityEC().AddOrUpdate(productByQuantity);
        }

        // Adds to cart with cartName
        [HttpPost("Carts/{cartName}")]
        public ProductByQuantity AddOrUpdate(string cartName, ProductByQuantity productByQuantity)
        {
            return new ProductByQuantityEC().AddOrUpdateCart(cartName, productByQuantity);

        }


    }
}
