using System;
using Library.GUI_App.Models;
using Library.GUI_App.Utilities;
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

            return true;
        }

        /* Expected function: Updates the quantity/ weight of the product in the cart with the given id.
         *                    Will also update the quantity/ weight in the inventory.
         * Expected input: Copy of product that exists in cart with the quantity to be removed from cart and added back to inventory.
         * Assumptions: Product definitely exists in cart. Quantity to be removed is valid (> 0 and <= quantity in cart).
         */
        public bool Delete(Product passedProduct)
        {
            var cartProduct = Products.Find(t => t.Id == passedProduct.Id);
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
                    ProductByWeight inventoryProductByWeight = (ProductByWeight)InventoryService.Products.Where(t => t.Id == cartProductByWeight.Id).First();
                    inventoryProductByWeight.Weight += (passedProduct as ProductByWeight).Weight;
                    cartProductByWeight.Weight -= (passedProduct as ProductByWeight).Weight;
                    if (cartProductByWeight.Weight <= 0) // if the weight is 0 or less, remove the product from the cart
                    {
                        Products.Remove(cartProductByWeight);
                    }
                }
                else if (cartProduct is ProductByQuantity cartProductByQuantity)
                {
                    ProductByQuantity inventoryProductByQuantity = (ProductByQuantity)InventoryService.Products.Where(t => t.Id == cartProductByQuantity.Id).First();
                    inventoryProductByQuantity.Quantity += (passedProduct as ProductByQuantity).Quantity;
                    cartProductByQuantity.Quantity -= (passedProduct as ProductByQuantity).Quantity;
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

        public bool Load(string filename = "defaultcart.json")
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            
            try
            {
                var inventoryJson = File.ReadAllText(persistPath + filename);
                productList = JsonConvert.DeserializeObject<List<Product>>(inventoryJson, options) ?? new List<Product>();
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
            foreach(Product product in Products)
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
