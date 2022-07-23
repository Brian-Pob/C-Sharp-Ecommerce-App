using CIS_Proj.API.Database;
using Library.GUI_App.Models;

namespace CIS_Proj.API.EC
{
    public class CartEC
    {
        public void Create(string cartName)
        {
            Filebase.Current.CreateCart(cartName);
        }
        public void Delete(string cartName)
        {
            Filebase.Current.DeleteCart(cartName);
        }
    }
}
