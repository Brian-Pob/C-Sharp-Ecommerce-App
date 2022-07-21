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
            List<ProductByWeight> tempList2 = new List<ProductByWeight>();
            tempList.ForEach(p =>
            {
                if (p is ProductByWeight)
                {
                    tempList2.Add(p as ProductByWeight);
                }
            });
            return tempList2;
        }
    }
}
