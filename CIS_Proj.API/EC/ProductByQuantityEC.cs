using CIS_Proj.API.Database;
using Library.GUI_App.Models;

namespace CIS_Proj.API.EC
{
    public class ProductByQuantityEC
    {
        public List<ProductByQuantity> Get()
        {
            return FakeProductDatabase.QuantityProducts;
        }

        public List<ProductByQuantity> Get(string name)
        {
            var tempList = FakeProductDatabase.Carts[name];
            List<ProductByQuantity> tempList2 = new List<ProductByQuantity>();
            tempList.ForEach(p =>
            {
                if (p is ProductByQuantity)
                {
                    tempList2.Add(p as ProductByQuantity);
                }
            });
            return tempList2;
        }

        public ProductByQuantity AddOrUpdate(string cartName, ProductByQuantity productByQuantity)
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
            var cartProduct = currentCart.FirstOrDefault(p => p.Id == productByQuantity.Id);
            // If the product is found in the cart, update the count
            // If the product is not found, add it to the list
            if (cartProduct != null)
            {
                currentCart.Remove(cartProduct);

                if (productByQuantity.Quantity != 0)
                    currentCart.Add(productByQuantity);
            }
            else
            {
                currentCart.Add(productByQuantity);
            }
            // Now the list in the Database is updated
            return productByQuantity;
        }
    }
}
