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
            var weightProductsJson = new WebRequestHandler().Get("http://localhost:5017/api/ProductByWeight").Result;
            productList = JsonConvert.DeserializeObject<List<Product>>(weightProductsJson);
            var quantityProductsJson = new WebRequestHandler().Get("http://localhost:5017/api/ProductByQuantity").Result;
            productList.AddRange(JsonConvert.DeserializeObject<List<Product>>(quantityProductsJson));

            //int qcount = 0, wcount = 0;
            //foreach (Product p in productList)
            //{
            //    if (p is ProductByQuantity)
            //    {
            //        // do something
            //        qcount++;
            //    }
            //    else if (p is ProductByWeight)
            //    {
            //        // do something
            //        wcount++;
            //    }
            //}
            //productList = new List<Product>();
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
        public bool AddOrUpdate(Product product) // Does Add and Update
        {
            //if (product == null || string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.Description) || product.Price < 0 ||
            //((product as ProductByWeight)?.Weight ?? (product as ProductByQuantity)?.Quantity) < 0)
            //{
            //    return false;
            //}
            
            if (product is ProductByWeight)
            {
                var response = new WebRequestHandler().Post("http://localhost:5017/api/ProductByWeight/AddOrUpdate", product).Result;
                var newProduct = JsonConvert.DeserializeObject<Product>(response);

                var old = Products.FirstOrDefault(t => t.Id == newProduct.Id);
                if (old != null)
                {
                    var index = productList.IndexOf(old);
                    productList.RemoveAt(index);
                    productList.Insert(index, newProduct);
                    return true;
                }
                else
                {
                    productList.Add(newProduct);
                    return true;
                }
            }
            else if (product is ProductByQuantity)
            {
                var response = new WebRequestHandler().Post("http://localhost:5017/api/ProductByQuantity/AddOrUpdate", product).Result;
                var newProduct = JsonConvert.DeserializeObject<Product>(response);

                var old = Products.FirstOrDefault(t => t.Id == newProduct.Id);
                if (old != null)
                {
                    var index = productList.IndexOf(old);
                    productList.RemoveAt(index);
                    productList.Insert(index, newProduct);
                    return true;
                }
                else
                {
                    productList.Add(newProduct);
                    return true;
                }
            }
            return false;
        }

        // Not currently in use but will keep for now just in case
        //public bool Update(Product product)
        //{
        //    for(int i = 0; i < Products.Count; i++)
        //    {
        //        if(Products[i].Id == product.Id)
        //        {
        //            Console.WriteLine("Found matching product. Updating information.");
        //            Products[i] = product;
        //            return true;
        //        }
        //    }
        //    Console.WriteLine("Product ID not found. Nothing was updated.");
        //    return false;
        //}

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
                var weightProductsJson = new WebRequestHandler().Get($"http://localhost:5017/api/ProductByWeight").Result;
                productList = JsonConvert.DeserializeObject<List<Product>>(weightProductsJson);
                var quantityProductsJson = new WebRequestHandler().Get($"http://localhost:5017/api/ProductByQuantity").Result;
                productList.AddRange(JsonConvert.DeserializeObject<List<Product>>(quantityProductsJson));
                
                return true;
            }
            catch (Exception)
            {

                //throw;
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
    }
}
