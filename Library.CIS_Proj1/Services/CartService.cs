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
                product.Id = NextId;
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

            return true;
        }
        

        public bool Delete(Product product)
        {
            foreach(Product p in Products)
            {
                if(p.Id == product.Id)
                {
                    Console.WriteLine("Found product in cart");
                    //if(p.Quantity < product.Quantity)
                    //{
                    //    Console.WriteLine("Qty to be removed exceeds existing qty. Removing all from cart.");
                    //    product.Quantity = p.Quantity;
                    //}

                    //p.Quantity -= product.Quantity;
                    //if(p.Quantity == 0)
                    //{
                    //    Console.WriteLine("Cart product quantity is 0. Product completely removed from cart.");
                    //    Products.Remove(p);
                    //}

                    //foreach(Product p2 in inventoryService.Products)
                    //{
                    //    if(p2.Id == product.Id)
                    //    {
                    //        Console.WriteLine("Updating inventory qty");
                    //        p2.Quantity += product.Quantity;
                    //        break;
                    //    }
                    //}

                    Console.WriteLine("Product qty removed from cart.");
                    return true;
                }
            }
            Console.WriteLine("Product not found in cart.");
            return false;
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

        public List<Product> Search(string term)
        {
            List<Product> foundProducts = new List<Product>();
            foreach (Product product in Products)
            {
                if (product.Name.ToLower().Contains(term.ToLower()) || product.Description.ToLower().Contains(term.ToLower()))
                {
                    foundProducts.Add(product.Clone());
                }
            }
            return foundProducts;
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

        
    }
}
