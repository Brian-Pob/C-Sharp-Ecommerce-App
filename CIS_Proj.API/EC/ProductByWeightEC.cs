using CIS_Proj.API.Database;
using Library.GUI_App.Models;

namespace CIS_Proj.API.EC
{
    public class ProductByWeightEC
    {
        public List<ProductByWeight> Get()
        {
            return FakeProductDatabase.WeightProducts;
        }

        public List<ProductByWeight> Get(string name)
        {
            var tempList = FakeProductDatabase.Carts[name];
            List<ProductByWeight> tempList2 = null;

            if (tempList == null) // if list does not exist, return null
                return tempList2;

            tempList2 = new List<ProductByWeight>();
            tempList.ForEach(p =>
            {
                if (p is ProductByWeight)
                {
                    tempList2.Add(p as ProductByWeight);
                }
            });
            return tempList2; // if no Products by weight, tempList2 is not null but Count == 0
        }

        public ProductByWeight AddOrUpdate(string cartName, ProductByWeight productByWeight)
        {
            // POST to api/ProdcutByWeight/Carts/{cartName}
            // This will search through the cartlist in the DB for the item
            var tempCart = Get(cartName);
            // Make sure to check that the Key and List pair exist
            // If the list does not exist, create it
            if (tempCart == null)
            {
                FakeProductDatabase.Carts.Add("cartName", new List<Product>());
            }

            // After the list is created, or if it already exists, search through the list for the passed product
            var currentCart = FakeProductDatabase.Carts[cartName];
            var cartProduct = currentCart.FirstOrDefault(p => p.Id == productByWeight.Id);
            // If the product is found in the cart, update the count
            // If the product is not found, add it to the list
            if (cartProduct != null)
            {
                currentCart.Remove(cartProduct);
                
                if(productByWeight.Weight != 0)
                    currentCart.Add(productByWeight);
            }
            else
            {
                currentCart.Add(productByWeight);
            }
            // Now the list in the Database is updated
            return productByWeight;
        }
    }
}
