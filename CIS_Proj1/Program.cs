using System;
using Library.CIS_Proj.Models;
using Library.CIS_Proj.Services;
using Library.CIS_Proj.Utilities;
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
                Console.WriteLine("\n-------------------------------\n");
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

                    _ = int.TryParse(Console.ReadLine() ?? "0", out int sortBy);
                    inventoryService.SortBy = sortBy;
                    ListNavigator<Product> navigator = inventoryService.ListNavigator;
                    UseNavigator(navigator);
                }
                else if (action == ActionType.SearchInventory)
                {
                    Console.WriteLine("--- Searching Inventory ---");
                    Console.Write("Enter term to search for:  ");
                    string term = Console.ReadLine() ?? "";

                    Console.WriteLine("Sort By: ");
                    Console.WriteLine("0 - Id (Default)");
                    Console.WriteLine("1 - Name");
                    Console.WriteLine("2 - Unit Price");

                    _ = int.TryParse(Console.ReadLine() ?? "0", out int sortBy);
                    
                    inventoryService.SortBy = sortBy;
                    _ = inventoryService.Search(term);

                    ListNavigator<Product> navigator = inventoryService.SearchListNavigator;
                    UseNavigator(navigator);

                }
                else if (action == ActionType.AddToCart)
                {
                    Console.WriteLine("--- Adding to Cart ---");
                    Console.Write("Enter Id of item to add to cart (blank to go back): ");
                    string idInput = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(idInput))
                    {
                        Console.WriteLine("Going back. Nothing added to cart.");
                        continue;
                    }
                    if (!int.TryParse(idInput, out int id))
                    {
                        Console.WriteLine("Invalid Id. Nothing added to cart.");
                        continue;
                    }
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

                    _ = int.TryParse(Console.ReadLine() ?? "0", out int sortBy);
                    cartService.SortBy = sortBy;
                    ListNavigator<Product> navigator = cartService.ListNavigator;
                    UseNavigator(navigator);
                }
                else if (action == ActionType.DeleteFromCart)
                {
                    Console.WriteLine("--- Deleting from Cart ---");
                    Console.Write("Enter Id of item to delete from cart: ");
                    _ = int.TryParse(Console.ReadLine() ?? "-1", out int id);
                    var product = cartService.Products.Find(p => p.Id == id);
                    if (product == null)
                    {
                        Console.WriteLine($"No item found with Id {id}.");
                    }
                    else
                    {
                        product = product.Clone();
                        Console.WriteLine($"Deleting {product.Name} from cart. " +
                            $"You have {(product as ProductByQuantity)?.Quantity ?? (product as ProductByWeight)?.Weight}" +
                            $"{(product is ProductByWeight ? " lbs" : "")} in your cart.");
                        if (product is ProductByQuantity)
                        {
                            Console.Write("Enter quantity to delete from cart: ");
                            _ = int.TryParse(Console.ReadLine() ?? "0", out int quantity);
                            ((ProductByQuantity)product).Quantity = quantity;
                        }
                        else if (product is ProductByWeight)
                        {
                            Console.Write("Enter weight to delete from cart: ");
                            _ = decimal.TryParse(Console.ReadLine() ?? "0", out decimal weight);
                            ((ProductByWeight)product).Weight = weight;
                        }

                        if (cartService.Delete(product))
                        {
                            Console.WriteLine($"Deleted {product.Name} from cart.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to delete {product.Name} from cart.");
                        }
                    }
                }
                else if (action == ActionType.SearchCart)
                {
                    Console.WriteLine("--- Searching Cart ---");
                    Console.Write("Enter term to search for:  ");
                    string term = Console.ReadLine() ?? "";

                    Console.WriteLine("Sort By: ");
                    Console.WriteLine("0 - Id (Default)");
                    Console.WriteLine("1 - Name");
                    Console.WriteLine("2 - Total Price");

                    _ = int.TryParse(Console.ReadLine() ?? "0", out int sortBy);

                    cartService.SortBy = sortBy;
                    _ = cartService.Search(term);

                    ListNavigator<Product> searchListNavigator = cartService.SearchListNavigator;
                    UseNavigator(searchListNavigator);
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
                }

                /* Employee Only Actions */
                if (login != LoginType.Employee)
                    continue;

                if (action == ActionType.AddToInventory)
                {
                    Product newProduct;
                    Console.WriteLine("--- Adding to Inventory ---");
                    var productType = SelectProductType();
                    if (productType == ProductType.Quantity)
                    {
                        Console.WriteLine("--- Adding Product By Quantity ---");
                        newProduct = new ProductByQuantity();
                        if (!FillQuantityProduct((ProductByQuantity)newProduct))
                        {
                            Console.WriteLine("Failed to add product.");
                            continue;
                        }
                        
                        if (inventoryService.Create(newProduct))
                        {
                            Console.WriteLine("Product added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Product could not be added.");
                        }
                    }
                    else if (productType == ProductType.Weight)
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
                    else if (productType == null)
                    {
                        Console.WriteLine("Invalid Product Type chosen.");
                    }
                }
                else if (action == ActionType.UpdateInventory)
                {
                    Console.WriteLine("--- Updating Inventory ---");
                    Console.Write("Enter Product Id to update (blank to go back): ");
                    string idInput = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(idInput))
                    {
                        Console.WriteLine("Cancelling update. Nothing was changed.");
                        continue;
                    }
                    if (!int.TryParse(idInput, out int id))
                    {
                        Console.WriteLine("Invalid Product Id. Cancelling update. Nothing was changed.");
                        continue;
                    }
                    var product = inventoryService.Products.FirstOrDefault(p => p.Id == id);
                    if (product == null)
                    {
                        Console.WriteLine("Product not found. Cancelling update. Nothing was changed.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Updating {product.Name}");
                        product = product.Clone();
                    }
                    
                    if (product is ProductByQuantity)
                    {
                        Console.WriteLine("Current Quantity: " + ((ProductByQuantity)product).Quantity);
                        Console.Write("Enter new quantity (blank to keep quantity): ");
                        string input2 = Console.ReadLine() ?? "";
                        int quantity = -1;
                        if (string.IsNullOrWhiteSpace(input2))
                        {
                            Console.WriteLine("Keeping current quantity.");
                        }
                        else
                        {
                            if(!int.TryParse(input2, out quantity))
                            {
                                Console.WriteLine("Invalid quantity.");
                                continue;
                            }
                            if (quantity < 0)
                            {
                                Console.WriteLine("Quantity cannot be negative.");
                                continue;
                            }
                            ((ProductByQuantity)product).Quantity = quantity;
                        }
                    }
                    else if (product is ProductByWeight)
                    {
                        Console.WriteLine("Current Weight: " + ((ProductByWeight)product).Weight);
                        Console.Write("Enter new weight (blank to keep weight): ");
                        string input2 = Console.ReadLine() ?? "";
                        decimal weight = -1;
                        if (string.IsNullOrWhiteSpace(input2))
                        {
                            Console.WriteLine("Keeping current weight.");
                        }
                        else
                        {
                            _ = decimal.TryParse(input2, out weight);
                            if (weight < 0)
                            {
                                Console.WriteLine("Weight cannot be negative.");
                                continue;
                            }
                            ((ProductByWeight)product).Weight = weight;
                        }
                    }

                    Console.Write("Update other details (name, desc, price, BOGO status)? (y/n): ");
                    var input = Console.ReadLine() ?? "";
                    if (input.ToLower() == "y")
                    {
                        string input2;
                        Console.Write("Enter name (blank to keep name): ");
                        input2 = Console.ReadLine() ?? "";
                        product.Name = (string.IsNullOrWhiteSpace(input2) ? product.Name : input2);
                        Console.Write("Enter description (blank to keep description): ");
                        input2 = Console.ReadLine() ?? "";
                        product.Description = (string.IsNullOrWhiteSpace(input2) ? product.Description : input2);
                        Console.Write("Enter price (blank to keep price): $");
                        input2 = Console.ReadLine() ?? "";
                        if (decimal.TryParse(input2, out decimal price))
                        {
                            if (price < 0)
                            {
                                Console.WriteLine("Price cannot be negative.");
                                continue;
                            }
                            product.Price = price;
                        }

                        Console.Write("Is item BOGO? (y/n): ");
                        input = Console.ReadLine() ?? "";
                        if (input.ToLower() == "y")
                        {
                            product.IsBogo = true;
                        }
                        else if (input.ToLower() == "n")
                        {
                            product.IsBogo = false;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input.");
                        }

                        Console.WriteLine("New product details:");
                        Console.WriteLine(product);
                        Console.Write("Confirm update? (y/n): ");
                        input = Console.ReadLine() ?? "";
                        if (input.ToLower() != "y")
                        {
                            Console.WriteLine("Update cancelled. No changes made.");
                            continue;
                        }

                        if (inventoryService.Update(product))
                        {
                            Console.WriteLine("Product updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Product could not be updated.");
                        }
                    }

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
        public static bool FillQuantityProduct(ProductByQuantity product)
        {
            if (product == null)
            {
                return false;
            }

            Console.Write("Item name: ");
            product.Name = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                Console.WriteLine("Invalid name.");
                return false;
            }
            
            Console.Write("Item Description: ");
            product.Description = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(product.Description))
            {
                Console.WriteLine("Invalid description.");
                return false;
            }

            Console.Write("Item price: $");
            if(!decimal.TryParse(Console.ReadLine() ?? "0", out decimal price))
            {
                Console.WriteLine("Invalid price.");
                return false;
            }
            if (price < 0)
            {
                Console.WriteLine("Price cannot be negative.");
                return false;
            }
            product.Price = price;

            Console.Write("Item quantity: ");
            if(!int.TryParse(Console.ReadLine() ?? "0", out int quantity))
            {
                Console.WriteLine("Invalid quantity.");
                return false;
            }
            if (quantity < 0)
            {
                Console.WriteLine("Invalid quantity.");
                return false;
            }
            product.Quantity = quantity;

            Console.Write("BOGO deal? (y/n): ");
            var input = Console.ReadLine() ?? "";
            if (input.ToLower() == "y")
            {
                product.IsBogo = true;
            }
            else if (input.ToLower() == "n")
            {
                product.IsBogo = false;
            }
            else
            {
                Console.WriteLine("Invalid input.");
                return false;
            }
            return true;
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

            Console.Write("BOGO deal? (y/n): ");
            var input = Console.ReadLine() ?? "";
            (product as ProductByWeight).IsBogo = input.ToLower() == "y";
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
            Console.Write("Enter billing information: ");
            Console.ReadLine();
            Console.Write("Enter shipping information: ");
            Console.ReadLine();

            Console.Write("\nConfirm checkout? (y/n): ");
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

        public static void UseNavigator(ListNavigator<Product> navigator)
        {
            while (true)
            {
                try
                {
                    foreach (var keyValuePair in navigator.GetCurrentPage())
                    {
                        Console.WriteLine(keyValuePair.Value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.Write("\nPages: ");
                for (int i = 1; i <= navigator.ListSize / navigator.PageSize + (navigator.ListSize % navigator.PageSize != 0 ? 1 : 0); i++)
                {
                    if (i == navigator.CurrentPage)
                    {
                        Console.Write($"-{i}- ");
                    }
                    else
                    {
                        Console.Write($"{i} ");
                    }
                }
                Console.WriteLine();
                if (navigator.HasNextPage) { Console.WriteLine("Next Page (N)"); }
                if (navigator.HasPreviousPage) { Console.WriteLine("Previous Page (P)"); }
                if (navigator.ListSize / navigator.PageSize > 0) { Console.WriteLine("Go to Page (Page#)"); }
                Console.WriteLine("Exit (E)");
                var input = Console.ReadLine() ?? "";
                if (input.ToLower() == "e") { break; }
                else if (input.ToLower() == "n" && navigator.HasNextPage) { navigator.GoForward(); }
                else if (input.ToLower() == "p" && navigator.HasPreviousPage) { navigator.GoBackward(); }
                else if (int.TryParse(input, out int page))
                {
                    try
                    {
                        navigator.GoToPage(page);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else { Console.WriteLine("Invalid Input"); }
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

    public enum ProductType
    {
        Quantity, Weight
    }
}