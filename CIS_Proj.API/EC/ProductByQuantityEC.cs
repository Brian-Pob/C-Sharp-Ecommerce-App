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
    }
}
