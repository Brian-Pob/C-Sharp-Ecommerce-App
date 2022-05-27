using System;
using Library.CIS_Proj1.Models;
using Newtonsoft.Json;
namespace Library.CIS_Proj1.Services
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

		private List<Item> itemList;
		public List<Item> Items
        {
			get
            {
				return itemList;
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
        
		public InventoryService()
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
            item.Id = NextId;
            Items.Add(item);
            return true;
        }

        public bool Update(Item item)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                if(Items[i].Id == item.Id)
                {
                    Console.WriteLine("Found matching item. Updating information.");
                    item.Id = Items[i].Id;
                    Items[i] = item;
                    return true;
                }
            }
            Console.WriteLine("Item ID not found. Nothing was updated.");
            return false;
        }

        public bool Save(string filename)
        {
            var inventoryJson = JsonConvert.SerializeObject(itemList);
            File.WriteAllText(filename, inventoryJson);
            return true;
        }

        public bool Load(string filename)
        {
            var inventoryJson = File.ReadAllText(filename);
            itemList = JsonConvert.DeserializeObject<List<Item>>(inventoryJson) ?? new List<Item>();
            return true;
        }

        public void List()
        {
            foreach (Item item in Items)
            {
                Console.WriteLine(item);
            }
        }

        public List<Item> Search(string term)
        {
            List<Item> foundItems = new List<Item>();
            foreach(Item item in Items)
            {
                if(item.Name.Contains(term) || item.Description.Contains(term))
                {
                    foundItems.Add(item.Clone());
                }
            }
            return foundItems;
        }

        
    }
}
