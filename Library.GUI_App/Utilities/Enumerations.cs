using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GUI_App.Utilities
{
    internal class Enumerations
    {

    }

    public enum LoginType
    {
        Employee, Customer
    }

    public enum ActionType
    {
        AddToInventory, ListInventory, UpdateInventory, SearchInventory,
        AddToCart, ListCart, DeleteFromCart, SearchCart, Checkout,
        Save, Load,
        Exit
    }

    public enum ProductType
    {
        Quantity, Weight
    }
}
