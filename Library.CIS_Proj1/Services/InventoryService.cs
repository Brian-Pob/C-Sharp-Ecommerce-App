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
        public ListNavigator<Product> ListNavigator
        {
            get
            {
                return listNavigator;
            }
        }
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
            listNavigator = new ListNavigator<Product>(SortedList);
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
            listNavigator = new ListNavigator<Product>(SortedList);
            return true;
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
