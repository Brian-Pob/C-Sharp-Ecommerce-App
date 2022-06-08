using System;
using Library.CIS_Proj.Models;
using Library.CIS_Proj.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS_Proj
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
                
                if (action == ActionType.Exit)
                {
                    break;
                }
                else if (action == ActionType.ListInventory)
                {
                    Console.WriteLine("--- Listing Inventory ---");
                    
                    Console.WriteLine("Sort By: ");
                    Console.WriteLine("0 - Id (Default)");
                    Console.WriteLine("1 - Name");
                    Console.WriteLine("2 - Unit Price");

                    _ = int.TryParse(Console.ReadLine() ?? "0", out int choice);

                    inventoryService.List(choice);
                }
                else if (action == ActionType.SearchInventory)
                {
                    Console.WriteLine("--- Searching Inventory ---");
                    Console.Write("Enter term to search for:  ");
                    string term = Console.ReadLine() ?? "";
                    List<Product> foundItems = inventoryService.Search(term);

                    if (foundItems.Count == 0)
                    {
                        Console.WriteLine($"No items found in inventory with {term} in name or description.");
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
                else if (action == ActionType.AddToCart)
                {
                    Console.WriteLine("--- Adding to Cart ---");
                    Console.Write("Enter Id of item to add to cart: ");
                    _ = int.TryParse(Console.ReadLine() ?? "0", out int id);
                    var product = inventoryService.Products.Find(p => p.Id == id);
                    if (product == null)
                    {
                        Console.WriteLine($"No item found with Id {id}.");
                    }
                    else
                    {
                        product = product.Clone();
                        Console.WriteLine($"Adding {product.Name} to cart.");

                        if (product.GetType() == typeof(ProductByQuantity))
                        {
                            Console.Write("Enter quantity to add to cart: ");
                            _ = int.TryParse(Console.ReadLine() ?? "0", out int quantity);
                            ((ProductByQuantity)product).Quantity = quantity;
                        }
                        else if (product.GetType() == typeof(ProductByWeight))
                        {
                            Console.Write("Enter weight to add to cart: ");
                            _ = decimal.TryParse(Console.ReadLine() ?? "0", out decimal weight);
                            ((ProductByWeight)product).Weight = weight;
                        }
                        if (cartService.AddToCart(product))
                        {
                            Console.WriteLine($"Added {product.Name} to cart.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to add {product.Name} to cart.");
                        }
                    }

                }
                else if (action == ActionType.ListCart)
                {
                    Console.WriteLine("--- Listing Cart ---");
                    Console.WriteLine("Sort By: ");
                    Console.WriteLine("0 - Id (Default)");
                    Console.WriteLine("1 - Name");
                    Console.WriteLine("2 - Total Price");

                    _ = int.TryParse(Console.ReadLine() ?? "0", out int choice);

                    cartService.List(choice);
                    cartService.NavigateList();
                }
                else if (action == ActionType.DeleteFromCart)
                {
                    Console.WriteLine("--- Deleting from Cart ---");
                }
                else if (action == ActionType.SearchCart)
                {
                    Console.WriteLine("--- Searching Cart ---");
                    Console.Write("Enter term to search for: ");
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
                    Console.WriteLine("--- Saving Cart and Inventory Data ---");
                    Console.WriteLine("Saving Inventory and Cart");
                    inventoryService.Save();
                    cartService.Save();
                }
                else if (action == ActionType.Load)
                {
                    Console.WriteLine("--- Loading Cart and Inventory Data ---");
                    Console.WriteLine("Loading Inventory and Cart");
                    inventoryService.Load();
                    cartService.Load();
                }
                else if (action == ActionType.Checkout)
                {
                    Console.WriteLine("--- Checking Out ---");
                    if (Checkout(cartService))
                    {
                        Console.WriteLine("Exiting");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Checkout failed.");
                    }
                }

                /* Employee Only Actions */
                if (login != LoginType.Employee)
                    continue;
                
                if (action == ActionType.AddToInventory)
                {
                    Product newProduct;
                    Console.WriteLine("--- Adding to Inventory ---");
                    var productType = SelectProductType();
                    if(productType == ProductType.Quantity)
                    {
                        Console.WriteLine("--- Adding Product By Quantity ---");
                        newProduct = new ProductByQuantity();
                        FillQuantityProduct((ProductByQuantity)newProduct);
                        if (inventoryService.Create(newProduct))
                        {
                            Console.WriteLine("Product added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Product could not be added.");
                        }
                    }
                    else if(productType == ProductType.Weight)
                    {
                        Console.WriteLine("--- Adding Product By Weight ---");
                        newProduct = new ProductByWeight();
                        FillWeightProduct((ProductByWeight)newProduct);
                        if (inventoryService.Create(newProduct))
                        {
                            Console.WriteLine("Product added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Product could not be added.");
                        }
                    }
                    else if(productType == null)
                    {
                        Console.WriteLine("Invalid Product Type chosen.");
                    }
                }
                else if(action == ActionType.UpdateInventory)
                {
                    Console.WriteLine("--- Updating Inventory ---");
                }
                /* End Employee Only Actions */

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
            _ = int.TryParse(Console.ReadLine() ?? "-1", out int selection);

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

        public static ProductType? SelectProductType()
        {
            Console.WriteLine("--- Select Product Type ---");
            int i = 0;
            foreach (var productType in (ProductType[])Enum.GetValues(typeof(ProductType)))
            {
                Console.WriteLine($"{i++} - {productType}");
            }
            _ = int.TryParse(Console.ReadLine() ?? "-1", out int selection);

            if (selection >= Enum.GetNames(typeof(ProductType)).Length || selection < 0)
                return null;

            return (ProductType)selection;
        }
        public static void FillQuantityProduct(ProductByQuantity product)
        {
            if (product == null)
            {
                return;
            }

            Console.Write("Item name: ");
            product.Name = Console.ReadLine() ?? string.Empty;

            Console.Write("Item Description: ");
            product.Description = Console.ReadLine() ?? string.Empty;

            Console.Write("Item price: $");
            _ = decimal.TryParse(Console.ReadLine() ?? "0", out decimal price);
            product.Price = price;

            Console.Write("Item quantity: ");
            _ = int.TryParse(Console.ReadLine() ?? "0", out int quantity);
            product.Quantity = quantity;
        }

        public static void FillWeightProduct(ProductByWeight product)
        {
            if (product == null)
            {
                return;
            }

            Console.Write("Item name: ");
            product.Name = Console.ReadLine() ?? string.Empty;

            Console.Write("Item Description: ");
            product.Description = Console.ReadLine() ?? string.Empty;

            Console.Write("Item price per lb: $");
            _ = decimal.TryParse(Console.ReadLine() ?? "0", out decimal price);
            product.Price = price;

            Console.Write("Item weight: ");
            _ = decimal.TryParse(Console.ReadLine() ?? "0", out decimal weight);
            product.Weight = weight;
        }


        public static bool Checkout(CartService cartService)
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
                return true;
            }
            else
            {
                Console.WriteLine("Exiting checkout.");
                return false;
            }
        }

        public static List<string> EnumToList(Enum myEnum)
        {
            List<string> list = new List<string>();
            int i = 0;
            foreach (var item in Enum.GetValues(myEnum.GetType()))
            {
                list.Add($"{i++} - {item}");
            }
            return list;
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

    public enum ProductType
    {
        Quantity, Weight
    }
}