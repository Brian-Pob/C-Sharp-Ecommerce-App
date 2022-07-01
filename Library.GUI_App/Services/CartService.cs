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
        private ListNavigator<Product> listNavigator;
        public ListNavigator<Product> ListNavigator
        {
            get
            {
                return listNavigator;
            }
        }
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
                listNavigator = new ListNavigator<Product>(SortedList);
                return true;


            }
            catch (Exception)
            {

                //throw;
                return false;
            }

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
                total += (product is ProductByWeight ? (product as ProductByWeight).TotalPrice : (product as ProductByQuantity).TotalPrice);
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
                var listCopy = new List<Product>(SortedList);
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
                        return (Products.OrderBy(i => (i as ProductByQuantity)?.TotalPrice ?? (i as ProductByWeight)?.TotalPrice));
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
                    foundProducts = (foundProducts.OrderBy(i => (i as ProductByQuantity)?.TotalPrice ?? (i as ProductByWeight)?.TotalPrice)); break;
                case 0:
                default:
                    foundProducts = (foundProducts.OrderBy(t => t.Id)); break;
            }
            
            searchListNavigator = new ListNavigator<Product>(foundProducts);
            return foundProducts;
        }


    }
}
