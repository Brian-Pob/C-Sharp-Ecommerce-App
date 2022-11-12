using CIS_Proj.API.Database;
using Library.GUI_App.Models;

namespace CIS_Proj.API.EC
{
    public class ProductByWeightEC
    {
        public List<ProductByWeight> Get()
        {
            return Filebase.Current.Products.Where(p => p is ProductByWeight).Cast<ProductByWeight>().ToList();
        }

        // Get List<ProductByWeight> From Cart with cartName
        public List<ProductByWeight> GetCart(string cartName)
        {
            return Filebase.Current.Carts[cartName].Where(p => p is ProductByWeight).Cast<ProductByWeight>().ToList();
            
        }

        public ProductByWeight AddOrUpdate(ProductByWeight productByWeight)
        {
            // since Filebase assigns the ID, no need to assign ID here

            return Filebase.Current.AddOrUpdateInventory(productByWeight) as ProductByWeight ?? new ProductByWeight();

        }
        
        public ProductByWeight AddOrUpdateCart(string cartName, ProductByWeight productByWeight)
        {
            return Filebase.Current.AddOrUpdateCart(cartName, productByWeight) as ProductByWeight ?? new ProductByWeight();
            
        }
    }
}
