using System;
using Library.GUI_App.Models;
using Library.Standard.CIS_Proj.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Library.GUI_App.Services
{
	public class CartService
	{
		private InventoryService inventoryService;
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

		private static CartService current;
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
                ProductByWeight inventoryProductByWeight = (ProductByWeight)InventoryService.Products.FirstOrDefault(p => p.Id == productByWeight.Id);
                if (inventoryProductByWeight.Weight < productByWeight.Weight)
                {
                    Console.WriteLine("Not enough weight in inventory. Nothing added to cart.");
                    return false;
                }
                else
                {
                    inventoryProductByWeight.Weight -= productByWeight.Weight;
                    var response = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByWeight/AddOrUpdate", inventoryProductByWeight).Result;
                    var newProductByWeight = JsonConvert.DeserializeObject<ProductByWeight>(response);

                }
            }
            else if (product is ProductByQuantity)
            {
                ProductByQuantity productByQuantity = (ProductByQuantity)product;
                ProductByQuantity inventoryProductByQuantity = (ProductByQuantity)InventoryService.Products.FirstOrDefault(p => p.Id == productByQuantity.Id);
                if (inventoryProductByQuantity.Quantity < productByQuantity.Quantity)
                {
                    Console.WriteLine("Not enough quantity in inventory. Nothing added to cart.");
                    return false;
                }
                else
                {
                    inventoryProductByQuantity.Quantity -= productByQuantity.Quantity;
                    var response = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByQuantity/AddOrUpdate", inventoryProductByQuantity).Result;
                    var newProductByQuantity = JsonConvert.DeserializeObject<ProductByQuantity>(response);
                }
            }

            // Finished subtracting amount from inventory and updating inventory in database. Now to add to the cart productList

            var cartProduct = Products.Find(t => t.Id == product.Id); // if product does not exist in cart
            if (cartProduct == null)
            {
                if (product is ProductByWeight)
                {
                    var response = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByWeight/Carts/{CartName}", product).Result;
                   
                    var newProduct = JsonConvert.DeserializeObject<Product>(response);

                    productList.Add(newProduct);
                }
                else if (product is ProductByQuantity)
                {
                    var response = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByQuantity/Carts/{CartName}", product).Result;

                    var newProduct = JsonConvert.DeserializeObject<Product>(response);

                    productList.Add(newProduct);
                }
                Console.WriteLine("New product added successfully.");
            }

            else // if product already exists in cart
            {
                Console.WriteLine("Product already exists in cart.");
                if (product is ProductByWeight)
                {
                    Console.WriteLine("Updating weight.");
                    var response = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByWeight/Carts/{CartName}", product).Result;

                    var newProduct = JsonConvert.DeserializeObject<Product>(response);
                    (newProduct as ProductByWeight).Weight += (cartProduct as ProductByWeight).Weight;
                    productList.Remove(cartProduct);
                    productList.Add(newProduct);
                }
                else if (product is ProductByQuantity)
                {
                    Console.WriteLine("Updating quantity.");
                    var response = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByQuantity/Carts/{CartName}", product).Result;

                    var newProduct = JsonConvert.DeserializeObject<Product>(response);
                    (newProduct as ProductByQuantity).Quantity += (cartProduct as ProductByQuantity).Quantity;
                    productList.Remove(cartProduct);
                    productList.Add(newProduct);
                }
            }

            return true;
        }

        /* Expected function: Updates the quantity/ weight of the product in the cart with the given id.
         *                    Will also update the quantity/ weight in the inventory.
         * Expected input: Copy of product that exists in cart with the quantity to be removed from cart and added back to inventory.
         * Assumptions: Product definitely exists in cart. Quantity to be removed is valid (> 0 and <= quantity in cart).
         */
        public bool Delete(Product passedProduct)
        {
            var cartProduct = Products.FirstOrDefault(t => t.Id == passedProduct.Id);
            var debug = InventoryService.Current.Products;
            if (cartProduct == null)
            {
                Console.WriteLine("Product not found in cart.");
                return false;
            }
            else
            {
                // first cast to ProductByWeight or ProductByQuantity to get the quantity/ weight
                if (cartProduct is ProductByWeight cartProductByWeight)
                {
                    ProductByWeight inventoryProductByWeight = (ProductByWeight)InventoryService.Products.FirstOrDefault(t => t.Id == cartProductByWeight.Id);
                    inventoryProductByWeight.Weight += (passedProduct as ProductByWeight).Weight;
                    cartProductByWeight.Weight -= (passedProduct as ProductByWeight).Weight;

                    _ = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByWeight/Carts/{CartName}", cartProductByWeight).Result;
                    _ = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByWeight/AddOrUpdate", inventoryProductByWeight).Result;

                    if (cartProductByWeight.Weight <= 0) // if the weight is 0 or less, remove the product from the cart
                    {
                        Products.Remove(cartProductByWeight);
                    }
                }
                else if (cartProduct is ProductByQuantity cartProductByQuantity)
                {
                    ProductByQuantity inventoryProductByQuantity = (ProductByQuantity)InventoryService.Products.FirstOrDefault(t => t.Id == cartProductByQuantity.Id);
                    inventoryProductByQuantity.Quantity += (passedProduct as ProductByQuantity).Quantity;
                    cartProductByQuantity.Quantity -= (passedProduct as ProductByQuantity).Quantity;
                    
                    _ = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByQuantity/Carts/{CartName}", cartProductByQuantity).Result;
                    _ = new WebRequestHandler().Post($"http://localhost:5017/api/ProductByQuantity/AddOrUpdate", inventoryProductByQuantity).Result;

                    if (cartProductByQuantity.Quantity <= 0)
                    {
                        Products.Remove(cartProductByQuantity);
                    }
                }

            }
            return true;
        }

        public string persistPath { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Carts\\"; } }

        public bool Save(string filename = "defaultcart.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            try
            {
                var inventoryJson = JsonConvert.SerializeObject(productList, options);

                File.WriteAllText(persistPath + filename, inventoryJson);
                return true;
            }
            catch (Exception)
            {

                //throw;
                return false;
            }
        }

        private string _cartName;
        public string CartName
        {
            get
            {
                return _cartName;
            }

            set
            {
                _cartName = value;
            }
        }
        public bool Load(string cartName)
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            try
            {
                var weightProductsJson = new WebRequestHandler().Get($"http://localhost:5017/api/ProductByWeight/Carts/{cartName}").Result;
                productList = JsonConvert.DeserializeObject<List<Product>>(weightProductsJson);
                var quantityProductsJson = new WebRequestHandler().Get($"http://localhost:5017/api/ProductByQuantity/Carts/{cartName}").Result;
                productList.AddRange(JsonConvert.DeserializeObject<List<Product>>(quantityProductsJson));
                CartName = cartName;
                //var cartsJson = new WebRequestHandler().Get("http://localhost:5017/api/Carts").Result;
                //var cartsDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<Product>>>(cartsJson);
                //var selectedCart = cartsDictionary[cartName];
                //productList = selectedCart;
                return true;
            }
            catch (Exception)
            {

                //throw;
                return false;
            }

        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach(var product in Products)
            {
                total += (product is ProductByWeight ? (product as ProductByWeight).TotalPrice : (product as ProductByQuantity).TotalPrice);
            }

            return total;
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

            
            return foundProducts;
        }


    }
}
