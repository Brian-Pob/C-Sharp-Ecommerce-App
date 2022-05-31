using System;
using Library.CIS_Proj1.Models;
using Newtonsoft.Json;
namespace Library.CIS_Proj1.Services
{
	public class CartService
	{
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

		public CartService()
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
            if(product.Quantity <= 1)
            {
                Console.WriteLine("Invalid quantity supplied.");
                return false;
            }
            
            foreach(Product p in InventoryService.Products)
            {
                if(p.Id == product.Id)
                {
                    Console.WriteLine("found matching product");
                    if(product.Quantity > p.Quantity)
                    {
                        Console.WriteLine("Quantity requested is greater than supply. Adding all to cart.");
                        product.Quantity = p.Quantity;
                    }
                    if(product.Quantity == 0)
                    {
                        Console.WriteLine("No available supply. Nothing added to cart");
                        return false;
                    }

                    Console.WriteLine("Quantity subtracted from inventory. Added to cart");
                    p.Quantity -= product.Quantity;
                    var newProduct = p.Clone();
                    newProduct.Quantity = product.Quantity;

                    foreach(Product p2 in Products)
                    {
                        if(p2.Id == newProduct.Id)
                        {
                            Console.WriteLine("Product is already in cart. Updating cart quantity.");
                            p2.Quantity += newProduct.Quantity;
                            return true;
                        }
                    }

                    Products.Add(newProduct); 
                    return true;
                }
            }

            Console.WriteLine("No product found. Nothing added to cart.");
            return false;
        }

        public bool Delete(Product product)
        {
            foreach(Product p in Products)
            {
                if(p.Id == product.Id)
                {
                    Console.WriteLine("Found product in cart");
                    if(p.Quantity < product.Quantity)
                    {
                        Console.WriteLine("Qty to be removed exceeds existing qty. Removing all from cart.");
                        product.Quantity = p.Quantity;
                    }

                    p.Quantity -= product.Quantity;
                    if(p.Quantity == 0)
                    {
                        Console.WriteLine("Cart product quantity is 0. Product completely removed from cart.");
                        Products.Remove(p);
                    }

                    foreach(Product p2 in inventoryService.Products)
                    {
                        if(p2.Id == product.Id)
                        {
                            Console.WriteLine("Updating inventory qty");
                            p2.Quantity += product.Quantity;
                            break;
                        }
                    }

                    Console.WriteLine("Product qty removed from cart.");
                    return true;
                }
            }
            Console.WriteLine("Product not found in cart.");
            return false;
        }

        public bool Save(string filename)
        {
            var cartJson = JsonConvert.SerializeObject(productList);
            File.WriteAllText(filename, cartJson);
            return true;
        }

        public bool Load(string filename)
        {
            var cartJson = File.ReadAllText(filename);
            productList = JsonConvert.DeserializeObject<List<Product>>(cartJson) ?? new List<Product>();
            return true;
        }

        public void List()
        {
            foreach(Product product in Products)
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
