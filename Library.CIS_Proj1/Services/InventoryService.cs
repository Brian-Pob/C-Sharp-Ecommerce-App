using System;
using Library.CIS_Proj.Models;
using Newtonsoft.Json;
namespace Library.CIS_Proj.Services
{
	public class InventoryService
	{
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
        
		public InventoryService()
		{
            productList = new List<Product>();
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

        public bool Save(string filename)
        {
            var inventoryJson = JsonConvert.SerializeObject(productList);
            File.WriteAllText(filename, inventoryJson);
            return true;
        }

        public bool Load(string filename)
        {
            var inventoryJson = File.ReadAllText(filename);
            productList = JsonConvert.DeserializeObject<List<Product>>(inventoryJson) ?? new List<Product>();
            return true;
        }

        public void List()
        {
            foreach (Product product in Products)
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
