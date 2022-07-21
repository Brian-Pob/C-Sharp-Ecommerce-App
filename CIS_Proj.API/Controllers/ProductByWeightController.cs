﻿using CIS_Proj.API.Database;
using Library.GUI_App.Models;
using CIS_Proj.API.EC;
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
            return new ProductByWeightEC().Get();
        }

        // GET api/<ProductByWeightController>/5
        [HttpGet("Carts/{name}")]
        public List<ProductByWeight> Get(string name)
        {
            return new ProductByWeightEC().Get(name);
        }

        // POST api/<ProductByWeightController>
        [HttpPost("AddOrUpdate")]
        public ProductByWeight AddOrUpdate(ProductByWeight productByWeight)
        {
            if (productByWeight.Id == -1)
            {
                productByWeight.Id = FakeProductDatabase.NextId();
                FakeProductDatabase.WeightProducts.Add(productByWeight);
                return productByWeight;
            }

            var productToUpdate = FakeProductDatabase.WeightProducts.FirstOrDefault(p => p.Id == productByWeight.Id);
            if (productToUpdate != null)
            {
                FakeProductDatabase.WeightProducts.Remove(productToUpdate);
                FakeProductDatabase.WeightProducts.Add(productByWeight);
            }
            return productByWeight;
        }

        [HttpPost("Carts/{cartName}")]
        public ProductByWeight AddOrUpdate(string cartName, ProductByWeight productByWeight)
        {
            return new ProductByWeightEC().AddOrUpdate(cartName, productByWeight);

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
