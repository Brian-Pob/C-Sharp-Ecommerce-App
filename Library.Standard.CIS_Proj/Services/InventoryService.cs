using System;
using Library.GUI_App.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Standard.CIS_Proj.Utilities;
using System.IO;

namespace Library.GUI_App.Services
{
	public class InventoryService
	{
        private CartService cartService;
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

		private static InventoryService current;
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
            if (product == null || string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.Description) || product.Price < 0 || 
                ((product as ProductByQuantity)?.Quantity ?? (product as ProductByWeight)?.Weight) < 0)
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

        private string persistPath
            = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\";
        public bool Save(string filename = "inventory.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            try
            {
                var inventoryJson = JsonConvert.SerializeObject(productList, options);

                File.WriteAllText(persistPath+filename, inventoryJson);
                return true;
            }
            catch (Exception)
            {

                //throw;
                return false;
            }
        }

        public bool Load(string filename = "inventory.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All

            };
            try
            {
                var inventoryJson = File.ReadAllText(persistPath+filename);
                productList = JsonConvert.DeserializeObject<List<Product>>(inventoryJson, options) ?? new List<Product>();
                return true;


            }
            catch (Exception)
            {

                //throw new Exception("Failed to load inventory from file.");
                return false;
            }            
            
        }
        
        private string searchTerm;
        public string SearchTerm
        {
            get
            {
                return searchTerm;
            }

            set
            {
                searchTerm = value;
            }
        }
        public IEnumerable<Product> SearchResults
        {
            get
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return new List<Product>();
                }
                else
                {

                    var foundProducts = Products.Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()));
                    var temp = foundProducts;
                    foundProducts = foundProducts.Concat(Products.Where(t => t.Description.ToLower().Contains(searchTerm.ToLower()) && !temp.Contains(t)));
                    return foundProducts;
                }
            }
        }
        public IEnumerable<Product> Search(string term)
        {
            IEnumerable<Product> foundProducts;

            if (!string.IsNullOrEmpty(term))
            {
                searchTerm = term; // if new term is not blank, replace saved searchTerm with new term
            }

            if (!string.IsNullOrEmpty(searchTerm)) // if saved search term is not blank, use saved searchTerm
            {
                foundProducts = Products.Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()));
                var temp = foundProducts;
                foundProducts = foundProducts.Concat(Products.Where(t => t.Description.ToLower().Contains(searchTerm.ToLower()) && !temp.Contains(t)));
                return foundProducts;
            }

            //if searchTerm is blank, return nothing
            return null;
        }
    }
}
