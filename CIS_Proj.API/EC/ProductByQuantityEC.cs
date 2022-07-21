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
    }
}
