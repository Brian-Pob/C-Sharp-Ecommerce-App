using System;
using Library.CIS_Proj1.Models;
using Newtonsoft.Json;
namespace Library.CIS_Proj1.Services
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

		private List<Item> itemList;
		public List<Item> Items
        {
			get
            {
				return itemList;
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

		public CartService()
		{
			itemList = new List<Item>();
		}

        public int NextId
        {
            get
            {
                if (!Items.Any())
                {
                    return 0;
                }

                return Items.Select(t => t.Id).Max() + 1;
            }
        }

        /* CRUD methods */
        public bool Create(Item item)
        {
            if(item.Quantity <= 1)
            {
                Console.WriteLine("Invalid quantity supplied.");
                return false;
            }
            
            foreach(Item i in InventoryService.Items)
            {
                if(i.Id == item.Id)
                {
                    Console.WriteLine("found matching item");
                    if(item.Quantity > i.Quantity)
                    {
                        Console.WriteLine("Quantity requested is greater than supply. Adding all to cart.");
                        item.Quantity = i.Quantity;
                    }
                    if(item.Quantity == 0)
                    {
                        Console.WriteLine("No available supply. Nothing added to cart");
                        return false;
                    }

                    Console.WriteLine("Quantity subtracted from inventory. Added to cart");
                    i.Quantity -= item.Quantity;
                    var newItem = i.Clone();
                    newItem.Quantity = item.Quantity;

                    foreach(Item i2 in Items)
                    {
                        if(i2.Id == newItem.Id)
                        {
                            Console.WriteLine("Item is already in cart. Updating cart quantity.");
                            i2.Quantity += newItem.Quantity;
                            return true;
                        }
                    }

                    Items.Add(newItem); 
                    return true;
                }
            }

            Console.WriteLine("No item found. Nothing added to cart.");
            return false;
        }

        public bool Delete(Item item)
        {
            foreach(Item i in Items)
            {
                if(i.Id == item.Id)
                {
                    Console.WriteLine("Found item in cart");
                    if(i.Quantity < item.Quantity)
                    {
                        Console.WriteLine("Qty to be removed exceeds existing qty. Removing all from cart.");
                        item.Quantity = i.Quantity;
                    }

                    i.Quantity -= item.Quantity;
                    if(i.Quantity == 0)
                    {
                        Console.WriteLine("Cart item quantity is 0. Item completely removed from cart.");
                        Items.Remove(i);
                    }

                    foreach(Item i2 in inventoryService.Items)
                    {
                        if(i2.Id == item.Id)
                        {
                            Console.WriteLine("Updating inventory qty");
                            i2.Quantity += item.Quantity;
                            break;
                        }
                    }

                    Console.WriteLine("Item qty removed from cart.");
                    return true;
                }
            }
            Console.WriteLine("Item not found in cart.");
            return false;
        }

        public bool Save(string filename)
        {
            var cartJson = JsonConvert.SerializeObject(itemList);
            File.WriteAllText(filename, cartJson);
            return true;
        }

        public bool Load(string filename)
        {
            var cartJson = File.ReadAllText(filename);
            itemList = JsonConvert.DeserializeObject<List<Item>>(cartJson) ?? new List<Item>();
            return true;
        }

        public void List()
        {
            foreach(Item item in Items)
            {
                Console.WriteLine(item);
            }
        }

        public List<Item> Search(string term)
        {
            List<Item> foundItems = new List<Item>();
            foreach (Item item in Items)
            {
                if (item.Name.Contains(term) || item.Description.Contains(term))
                {
                    foundItems.Add(item.Clone());
                }
            }
            return foundItems;
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach(Item item in Items)
            {
                total += item.TotalPrice;
            }

            return total;
        }

        
    }
}
