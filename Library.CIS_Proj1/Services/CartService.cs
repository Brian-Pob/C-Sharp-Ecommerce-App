using System;
using Library.CIS_Proj.Models;
using Library.CIS_Proj.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.CIS_Proj.Services
{
	public class CartService
	{
        private ListNavigator<Product> listNavigator;
        public ListNavigator<Product> ListNavigator
        {
            get
            {
                return listNavigator;
            }
        }
		private InventoryService? inventoryService;
		public InventoryService InventoryService
		{
			get
			{
				return inventoryService;
			}

            set
            {
				inventoryService = value;
            }
		}

		private List<Product> productList;
		public List<Product> Products
        {
			get
            {
				return productList;
            }
        }

		private static CartService? current;
		public static CartService Current
        {
			get
            {
				if(current == null)
                {
					current = new CartService();
                }

				return current;
            }
        }

		private CartService()
		{
			productList = new List<Product>();
            listNavigator = new ListNavigator<Product>(productList);
        }

        public int NextId
        {
            get
            {
                if (!Products.Any())
                {
                    return 0;
                }

                return Products.Select(t => t.Id).Max() + 1;
            }
        }

        /* CRUD methods */
        public bool AddToCart(Product product)
        {
            if (product == null)
                return false;
            
            if (product is ProductByWeight)
            {
                if (((ProductByWeight)product).Weight <= 0)
                {
                    Console.WriteLine("Invalid weight");
                    return false;
                }
            }
            else if (product is ProductByQuantity)
            {
                if (((ProductByQuantity)product).Quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity");
                    return false;
                }
            }

            // Since we know product exists in inventory, we just have to subtract the quantity/ weight from the inventory item

            if (product is ProductByWeight)
            {
                ProductByWeight productByWeight = (ProductByWeight)product;
                ProductByWeight inventoryProductByWeight = (ProductByWeight)InventoryService.Products.Where(t => t.Id == productByWeight.Id).First();
                if (inventoryProductByWeight.Weight < productByWeight.Weight)
                {
                    Console.WriteLine("Not enough weight in inventory. Nothing added to cart.");
                    return false;
                }
                else
                {
                    inventoryProductByWeight.Weight -= productByWeight.Weight;
                }
            }
            else if (product is ProductByQuantity)
            {
                ProductByQuantity productByQuantity = (ProductByQuantity)product;
                ProductByQuantity inventoryProductByQuantity = (ProductByQuantity)InventoryService.Products.Where(t => t.Id == productByQuantity.Id).First();
                if (inventoryProductByQuantity.Quantity < productByQuantity.Quantity)
                {
                    Console.WriteLine("Not enough quantity in inventory. Nothing added to cart.");
                    return false;
                }
                else
                {
                    inventoryProductByQuantity.Quantity -= productByQuantity.Quantity;
                }
            }
            var cartProduct = Products.Find(t => t.Id == product.Id);
            if (cartProduct == null)
            {
                Products.Add(product);
                Console.WriteLine("New product added successfully.");
            }
            else
            {
                Console.WriteLine("Product already exists in cart.");
                if (product is ProductByWeight)
                {
                    Console.WriteLine("Updating weight.");
                    ((ProductByWeight)cartProduct).Weight += ((ProductByWeight)product).Weight;
                }
                else if (product is ProductByQuantity)
                {
                    Console.WriteLine("Updating quantity.");
                    ((ProductByQuantity)cartProduct).Quantity += ((ProductByQuantity)product).Quantity;
                }
            }
            try
            {
                Console.WriteLine("Debug: " + cartProduct);
            }
            catch (Exception e)
            {
                Console.WriteLine("Debug: " + e.Message);
            }

            return true;
        }
        
        /*
         * Expected input: Product with Id exists in cart but quantity may not be valid.
         */
        public bool Delete(Product product)
        {
            var cartProduct = Products.Find(t => t.Id == product.Id);
            if (cartProduct == null)
            {
                Console.WriteLine("Product not found in cart.");
                return false;
            }
            else
            {
                // Check if quantity/ weight is valid
                
                if (product is ProductByWeight)
                {
                    if (((ProductByWeight)product).Weight <= 0)
                    {
                        Console.WriteLine("Invalid weight. Nothing deleted from cart.");
                        return false;
                    }
                    else if (((ProductByWeight)cartProduct).Weight < ((ProductByWeight)product).Weight)
                    {
                        Console.WriteLine("Not enough weight in cart. Nothing deleted.");
                        return false;
                    }
                    

                    Console.WriteLine("Updating weight in cart.");
                    ((ProductByWeight)cartProduct).Weight -= ((ProductByWeight)product).Weight;
                    if (((ProductByWeight)cartProduct).Weight == 0)
                    {
                        Console.WriteLine("Weight is 0. Deleting product from cart.");
                        Products.Remove(cartProduct);
                    }
                    else
                    {
                        Console.WriteLine($"Weight of {cartProduct.Name} in cart is now {((ProductByWeight)cartProduct).Weight}.");
                    }
                }
                else if (product is ProductByQuantity)
                {
                    if (((ProductByQuantity)product).Quantity <= 0)
                    {
                        Console.WriteLine("Invalid quantity. Nothing deleted from cart.");
                        return false;
                    }
                    else if (((ProductByQuantity)cartProduct).Quantity < ((ProductByQuantity)product).Quantity)
                    {
                        Console.WriteLine("Not enough quantity in cart. Nothing deleted.");
                        return false;
                    }
                    
                    Console.WriteLine("Updating quantity in cart.");
                    ((ProductByQuantity)cartProduct).Quantity -= ((ProductByQuantity)product).Quantity;
                    if (((ProductByQuantity)cartProduct).Quantity == 0)
                    {
                        Console.WriteLine("Quantity is 0. Deleting product from cart.");
                        Products.Remove(cartProduct);
                    }
                    else
                    {
                        Console.WriteLine($"Quantity of {cartProduct.Name} in cart is now {((ProductByQuantity)cartProduct).Quantity}.");
                    }

                }
            }
            return true;
        }

        public bool Save(string filename = "cart.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var cartJson = JsonConvert.SerializeObject(productList, options);
            File.WriteAllText(filename, cartJson);
            return true;
        }

        public bool Load(string filename = "cart.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var cartJson = File.ReadAllText(filename);
            productList = JsonConvert.DeserializeObject<List<Product>>(cartJson, options) ?? new List<Product>();
            listNavigator = new ListNavigator<Product>(productList);
            return true;
        }

        public void List(int choice = 0)
        {
            string sortBy;
            switch (choice)
            {
                case 1: sortBy = "Name"; break;
                case 2: sortBy = "TotalPrice"; break;
                case 0:
                default: sortBy = "Id"; break;
            }

            if (sortBy.Equals("Id"))
            {
                foreach (Product product in Products)
                {
                    Console.WriteLine(product);
                    //Console.WriteLine("Debugging: " + product.GetType());
                }
            }
            else if (sortBy.Equals("Name"))
            {
                foreach (Product product in Products.OrderBy(t => t.Name))
                {
                    Console.WriteLine(product);
                    //Console.WriteLine("Debugging: " + product.GetType());
                }
            }
            else if (sortBy.Equals("TotalPrice"))
            {
                foreach (Product product in Products.OrderBy(t => t.TotalPrice))
                {
                    Console.WriteLine(product);
                    //Console.WriteLine("Debugging: " + product.GetType());
                }
            }
        }

        public void NavigateList()
        {
            var listWindow = listNavigator.GetCurrentPage();
            foreach (var product in listWindow)
            {
                Console.WriteLine(product);
            }
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach(Product product in Products)
            {
                total += product.TotalPrice;
            }

            return total;
        }

        private int _sortBy;
        public int SortBy
        {
            get
            {
                return _sortBy;
            }

            set
            {
                _sortBy = value;
                listNavigator = new ListNavigator<Product>(SortedList);
            }
        }
        public IEnumerable<Product> SortedList
        {
            get
            {
                switch (SortBy)
                {
                    case 1:
                        return (Products.OrderBy(t => t.Name));
                    case 2:
                        return (Products.OrderBy(t => t.Price));
                    case 0:
                    default:
                        return (Products.OrderBy(t => t.Id));
                }
            }
        }

        private string searchTerm;
        private ListNavigator<Product> searchListNavigator;
        public ListNavigator<Product> SearchListNavigator
        {
            get
            {
                return searchListNavigator;
            }
        }
        public IEnumerable<Product> Search(string term)
        {
            IEnumerable<Product> foundProducts;

            if (!string.IsNullOrEmpty(term))
            {
                searchTerm = term;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                foundProducts = Products.Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()));
                var temp = foundProducts;
                foundProducts = foundProducts.Concat(Products.Where(t => t.Description.ToLower().Contains(searchTerm.ToLower()) && !temp.Contains(t)));
            }
            else
            {
                foundProducts = Products;
            }

            switch (SortBy)
            {
                case 1:
                    foundProducts = (foundProducts.OrderBy(t => t.Name)); break;
                case 2:
                    foundProducts = (foundProducts.OrderBy(t => t.Price)); break;
                case 0:
                default:
                    foundProducts = (foundProducts.OrderBy(t => t.Id)); break;
            }
            searchListNavigator = new ListNavigator<Product>(foundProducts);
            return foundProducts;
        }


    }
}
