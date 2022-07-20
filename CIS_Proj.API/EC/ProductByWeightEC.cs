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
    }
}
