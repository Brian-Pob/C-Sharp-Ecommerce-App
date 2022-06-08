using System;
using Library.CIS_Proj.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.CIS_Proj.Utilities;

namespace Library.CIS_Proj.Services
{
	public class InventoryService
	{
        private ListNavigator<Product> listNavigator;
		private CartService? cartService;
		public CartService CartService
        {
			get
            {
				return cartService;
            }

            set
            {
				cartService = value;
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

		private static InventoryService? current;
		public static InventoryService Current
        {
            get
            {
				if(current == null)
                {
                    current = new InventoryService();
                }

                return current;
            }
        }
        
		private InventoryService()
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
        public bool Create(Product product)
        {
            if (product == null)
            {
                return false;
            }
            
            product.Id = NextId;
            Products.Add(product);
            return true;
        }

        public bool Update(Product product)
        {
            for(int i = 0; i < Products.Count; i++)
            {
                if(Products[i].Id == product.Id)
                {
                    Console.WriteLine("Found matching product. Updating information.");
                    Products[i] = product;
                    return true;
                }
            }
            Console.WriteLine("Product ID not found. Nothing was updated.");
            return false;
        }

        public bool UpdateProductQuantity(ProductByQuantity product)
        {
            for(int i = 0; i < Products.Count; i++)
            {
                if(Products[i].Id == product.Id)
                {
                    Console.WriteLine("Found matching product. Updating Quantity.");
                    ((ProductByQuantity) Products[i]).Quantity = product.Quantity;
                    return true;
                }
            }

            Console.WriteLine("Failed to update item quantity. Nothing was updated.");
            return false;
        }

        public bool Save(string filename = "inventory.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var inventoryJson = JsonConvert.SerializeObject(productList, options);
            File.WriteAllText(filename, inventoryJson);
            return true;
        }

        public bool Load(string filename = "inventory.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var inventoryJson = File.ReadAllText(filename);
            productList = JsonConvert.DeserializeObject<List<Product>>(inventoryJson, options) ?? new List<Product>();
            listNavigator = new ListNavigator<Product>(productList);
            return true;
        }

        public void List(int choice = 0)
        {
            string sortBy;
            switch (choice)
            {
                case 1: sortBy = "Name"; break;
                case 2: sortBy = "Price"; break;
                case 0:
                default: sortBy = "Id"; break;
            }
            
            if(sortBy.Equals("Id"))
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
            else if (sortBy.Equals("Price"))
            {
                foreach (Product product in Products.OrderBy(t => t.Price))
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
            foreach(Product product in Products)
            {
                if(product.Name.ToLower().Contains(term.ToLower()) || product.Description.ToLower().Contains(term.ToLower()))
                {
                    foundProducts.Add(product.Clone());
                }
            }
            return foundProducts;
        }

        
    }
}
