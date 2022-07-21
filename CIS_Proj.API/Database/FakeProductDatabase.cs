using Library.GUI_App.Models;

namespace CIS_Proj.API.Database
{
    public class FakeProductDatabase
    {
        public static List<Product> Products
        {
            get
            {
                var returnList = new List<Product>();
                
                WeightProducts.ForEach(returnList.Add);
                QuantityProducts.ForEach(returnList.Add);

                return returnList;
            }
        }

        public static List<ProductByWeight> WeightProducts = new List<ProductByWeight>
        {
            new ProductByWeight { Id=0, Name="Apple", Description="Bag of apples", Weight=1, Price=1.00m},
            new ProductByWeight { Id=1, Name="Bananas", Description="Bag of bananas", Weight=2, Price=2.00m},
            new ProductByWeight { Id=2, Name="Carrots", Description="Bag of carrots", Weight=3, Price=1.50m},
        };

        public static List<ProductByQuantity> QuantityProducts = new List<ProductByQuantity>
        {
            new ProductByQuantity { Id=3, Name="Cheese Pizza", Description="Slice of cheese pizza", Quantity=1, Price=1.00m},
            new ProductByQuantity { Id=4, Name="Pepperoni Pizza", Description="Slice of pepperoni pizza", Quantity=3, Price=2.50m},
            new ProductByQuantity { Id=5, Name="Bagels", Description="Pack of bagels", Quantity=5, Price=1.99m},

        };

        public static int NextId()
        {
            if (!Products.Any())
            {
                return 1;
            }

            return Products.Select(t => t.Id).Max() + 1;

        }
    }
}
