using CIS_Proj.API.Database;
using Library.GUI_App.Models;

namespace CIS_Proj.API.EC
{
    public class ProductByQuantityEC
    {
        public List<ProductByQuantity> Get()
        {
            return Filebase.Current.Products.Where(p => p is ProductByQuantity).Cast<ProductByQuantity>().ToList();
        }

        // Get List<ProductByQuantity> From Cart with cartName
        public List<ProductByQuantity> GetCart(string cartName)
        {
            return Filebase.Current.Carts[cartName].Where(p => p is ProductByQuantity).Cast<ProductByQuantity>().ToList();

        }

        public ProductByQuantity AddOrUpdate(ProductByQuantity productByQuantity)
        {
            // since Filebase assigns the ID, no need to assign ID here

            return Filebase.Current.AddOrUpdateInventory(productByQuantity) as ProductByQuantity ?? new ProductByQuantity();

        }

        public ProductByQuantity AddOrUpdateCart(string cartName, ProductByQuantity productByQuantity)
        {
            return Filebase.Current.AddOrUpdateCart(cartName, productByQuantity) as ProductByQuantity ?? new ProductByQuantity();

        }
    }
}
