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

            Console.WriteLine($"You chose:  {login}.");

            bool cont = true;
            while (cont)
            {
                var action = SelectActionType(login);
                if(login == LoginType.Employee)
                {
                    if (action == ActionType.AddToInventory)
                    {
                        Console.WriteLine("Adding to inventory");
                        var newItem = new Product();
                        FillItem(newItem);
                        inventoryService.Create(newItem);
                    }
                    else if (action == ActionType.UpdateInventory)
                    {
                        Console.WriteLine("Updating Inventory");

                        var newItem = new Product();
                        

                        Console.Write("Updating quantity only? (Y/n");
                        if((Console.ReadLine() ?? "n").ToLower() == "y")
                        {
                            FillIdAndQuantity(newItem);
                            inventoryService.UpdateProductQuantity(newItem);
                        }
                        else
                        {
                            Console.Write("Enter new item data: ");
                            Console.Write("Item ID:  ");
                            _ = int.TryParse(Console.ReadLine() ?? "0", out int itemId);
                            newItem.Id = itemId;
                            FillItem(newItem);
                            inventoryService.Update(newItem);
                        }
                    }
                }

                if (action == ActionType.ListInventory)
                {
                    Console.WriteLine("Listing Inventory");
                    inventoryService.List();
                }
                else if (action == ActionType.SearchInventory)
                {
                    Console.WriteLine("Searching Inventory");
                    Console.Write("Enter term to search for:  ");
                    string term = Console.ReadLine() ?? "";
                    List<Product> foundItems = inventoryService.Search(term);

                    if(foundItems.Count == 0)
                    {
                        Console.WriteLine($"No items found in inventory with {term} in name or description.");
                    }
                    else
                    {
                        Console.WriteLine("Found items:  ");
                        foreach(Product item in foundItems)
                        {
                            Console.WriteLine(item);
                        }
                    }
                }
                else if (action == ActionType.AddToCart)
                {
                    Console.WriteLine("Adding to Cart");
                    var newItem = new Product();
                    if (FillIdAndQuantity(newItem))
                        cartService.Create(newItem);

                }
                else if (action == ActionType.ListCart)
                {
                    Console.WriteLine("Listing Cart");
                    cartService.List();

                }
                else if (action == ActionType.DeleteFromCart)
                {
                    Console.WriteLine("Deleting from Cart");
                    var newItem = new Product();
                    
                    if(FillIdAndQuantity(newItem))
                        cartService.Delete(newItem);
                }
                else if (action == ActionType.SearchCart)
                {
                    Console.WriteLine("Searching Cart");
                    Console.Write("Enter term to search for:  ");
                    string term = Console.ReadLine() ?? "";
                    List<Product> foundItems = cartService.Search(term);

                    if (foundItems.Count == 0)
                    {
                        Console.WriteLine($"No items found in cart with {term} in name or description.");
                    }
                    else
                    {
                        Console.WriteLine("Found items:  ");
                        foreach (Product item in foundItems)
                        {
                            Console.WriteLine(item);
                        }
                    }
                }
                else if (action == ActionType.Save)
                {
                    Console.WriteLine("Saving Inventory and Cart");
                    inventoryService.Save("inventory.json");
                    cartService.Save("cart.json");
                }
                else if (action == ActionType.Load)
                {
                    Console.WriteLine("Loading Inventory and Cart");
                    inventoryService.Load("inventory.json");
                    cartService.Load("cart.json");
                }
                else if (action == ActionType.Checkout)
                {
                    Console.WriteLine("Checking out");
                    Checkout(cartService);
                }
                else if (action == ActionType.Exit)
                {
                    Console.WriteLine("Exiting");
                    break;
                }

                Console.WriteLine("\n");
            }

            Console.WriteLine("End of program...");
        }

        public static LoginType SelectLoginType()
        {
            Console.WriteLine("Select Login Type:  ");
            int i = 0;
            foreach (LoginType lt in (LoginType[]) Enum.GetValues(typeof(LoginType)))
            {
                Console.WriteLine($"{i++} - {lt}");

            }
            _ = int.TryParse(Console.ReadLine() ?? "0", out int selection);

            if (selection >= Enum.GetNames(typeof(LoginType)).Length || selection < 0)
                selection = 0;

            return (LoginType)selection;
        }

        public static ActionType SelectActionType(LoginType login)
        {
            Console.WriteLine("Select Action Type:  ");
            int i = 0;
            foreach (ActionType action in (ActionType[])Enum.GetValues(typeof(ActionType)))
            {
                if(login == LoginType.Customer)
                {
                    if (!action.ToString().Contains("Inventory") || action.ToString().Contains("List") || action.ToString().Contains("Search"))
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
            _ = int.TryParse(Console.ReadLine() ?? "0", out int selection);

            if (selection >= Enum.GetNames(typeof(ActionType)).Length || selection < 0)
                selection = 0;

            return (ActionType)selection;
        }

        public static void FillItem(Product? item)
        {
            if(item == null)
            {
                return;
            }

            Console.Write("Item name: ");
            item.Name = Console.ReadLine() ?? string.Empty;

            Console.Write("Item Description: ");
            item.Description = Console.ReadLine() ?? string.Empty;

            Console.Write("Item price: ");
            // Discard variable _ indicates that we do not need the result
            // of the function.
            _ = decimal.TryParse(Console.ReadLine(), out decimal price);
            item.Price = price;

            Console.Write("Item quantity: ");
            _ = int.TryParse(Console.ReadLine(), out int quantity);
            item.Quantity = quantity;
        }

        public static bool FillIdAndQuantity(Product newItem)
        {
            Console.Write("Item number:  ");
            _ = int.TryParse(Console.ReadLine() ?? "0", out int itemId);
            newItem.Id = itemId;

            Console.Write("Item Quantity:  ");
            _ = int.TryParse(Console.ReadLine() ?? "0", out int quant);
            newItem.Quantity = quant;

            if (quant <= 0)
            {
                Console.WriteLine("Quantity must be greater than 0");
                return false;
            }

            return true;
        }

        public static void Checkout(CartService cartService)
        {
            Console.WriteLine("Your cart has the following items");
            cartService.List();
            decimal total = cartService.GetTotal();
            double salesTax = .075;
            Console.WriteLine("Subtotal:  $" + total);
            Console.WriteLine("7.5% Sales Tax:  $" + Decimal.Round((total * (decimal)salesTax), 2));
            Console.WriteLine("Total:  $" + Decimal.Round((total + total * (decimal)salesTax), 2));
            Console.Write("Confirm checkout? (Y/n):  ");
            var input = Console.ReadLine() ?? "n";
            if(input.ToLower() == "y")
            {
                cartService.Products.Clear();
                Console.WriteLine("Checked out. All items removed from cart.");
            }
            else
            {
                Console.WriteLine("Exiting checkout.");
            }
        }
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
}