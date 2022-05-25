using System;
using Library.CIS_Proj1.Models;
using Library.CIS_Proj1.Services;
using Newtonsoft.Json;

namespace CIS_Proj1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cartService = CartService.Current;
            var inventoryService = InventoryService.Current;
            cartService.InventoryService = inventoryService;
            inventoryService.CartService = cartService;

            Console.WriteLine("Welcome to the C# store.");

            LoginType login = SelectLoginType();

            Console.WriteLine($"You chose: {login}.");

            bool cont = true;
            while (cont)
            {
                var action = SelectActionType(login);
                if(login == LoginType.Employee)
                {
                    if (action == ActionType.AddToInventory)
                    {
                        Console.WriteLine("Adding to inventory");
                        var newItem = new Item();
                        FillItem(newItem);
                        inventoryService.Create(newItem);
                    }
                    else if (action == ActionType.ListInventory)
                    {
                        Console.WriteLine("Listing Inventory");
                        cartService.CartListInv();
                    }
                    else if (action == ActionType.UpdateInventory)
                    {
                        Console.WriteLine("Updating Inventory");
                        var newItem = new Item();

                        Console.WriteLine("Enter new item data:");
                        Console.WriteLine("Item ID: ");
                        _ = int.TryParse(Console.ReadLine() ?? "0", out int itemId);
                        newItem.Id = itemId;
                        FillItem(newItem);
                        inventoryService.Update(newItem);
                    }
                    else if (action == ActionType.SearchInventory)
                    {
                        Console.WriteLine("Searching Inventory");
                    }
                }
                

                if (action == ActionType.AddToCart)
                {
                    Console.WriteLine("Adding to Cart");
                    var newItem = new Item();
                    FillItem(newItem);
                    // get Inventory itemList
                    // check if item exists
                    // check if quantity is enough
                    // update cart with new quantity (only quantity should be updated)
                    // create new item in Cart
                    cartService.Create(newItem);
                }
                else if (action == ActionType.ListCart)
                {
                    Console.WriteLine("Listing Cart");
                    inventoryService.InvListCart();

                }
                else if (action == ActionType.DeleteFromCart)
                {
                    Console.WriteLine("Deleting from Cart");
                }
                else if (action == ActionType.SearchCart)
                {
                    Console.WriteLine("Searching Cart");
                }
                else if (action == ActionType.Exit)
                {
                    Console.WriteLine("Exiting");
                    break;
                }
            }

            Console.WriteLine("End of program...");
        }

        public static LoginType SelectLoginType()
        {
            Console.WriteLine("Select Login Type: ");
            int i = 0;
            foreach (LoginType lt in (LoginType[]) Enum.GetValues(typeof(LoginType)))
            {
                Console.WriteLine($"{i++} - {lt}");

            }
            int selection = int.Parse(Console.ReadLine() ?? "0");

            if (selection >= Enum.GetNames(typeof(LoginType)).Length || selection < 0)
                selection = 0;

            return (LoginType)selection;
        }

        public static ActionType SelectActionType(LoginType login)
        {
            Console.WriteLine("Select Action Type: ");
            int i = 0;
            foreach (ActionType action in (ActionType[])Enum.GetValues(typeof(ActionType)))
            {
                if(login == LoginType.Customer)
                {
                    if (!action.ToString().Contains("Inventory"))
                    {
                        Console.WriteLine($"{i} - {action}");
                    }
                }
                else if(login == LoginType.Employee)
                {
                    Console.WriteLine($"{i} - {action}");
                }
                i++;
            }
            int selection = int.Parse(Console.ReadLine() ?? "0");

            if (selection >= Enum.GetNames(typeof(ActionType)).Length || selection < 0)
                selection = 0;

            return (ActionType)selection;
        }

        public static void FillItem(Item? item)
        {
            if(item == null)
            {
                return;
            }

            Console.WriteLine("Item name:");
            item.Name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Item Description:");
            item.Description = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Item price:");
            // Discard variable _ indicates that we do not need the result
            // of the function.
            _ = float.TryParse(Console.ReadLine(), out float price);
            item.Price = price;

            Console.WriteLine("Item quantity:");
            _ = int.TryParse(Console.ReadLine(), out int quantity);
            item.Quantity = quantity;
        }
    }

    public enum LoginType
    {
        Employee, Customer
    }

    public enum ActionType
    {
        AddToInventory, ListInventory, UpdateInventory, SearchInventory,
        AddToCart, ListCart, DeleteFromCart, SearchCart,
        Exit
    }
}