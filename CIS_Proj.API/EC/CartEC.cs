using CIS_Proj.API.Database;
using Library.GUI_App.Models;

namespace CIS_Proj.API.EC
{
    public class CartEC
    {
        public void Create(string cartName)
        {
            FakeProductDatabase.Carts.Add(cartName, new List<Product>());
        }
        public void Delete(string cartName)
        {
            FakeProductDatabase.Carts.Remove(cartName);
        }
    }
}
