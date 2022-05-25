using System;
using Library.CIS_Proj1.Models;
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
                    return 1;
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

        /* Testing methods */
        public bool CartListInv()
        {
            var invItemList = InventoryService.Items;
            Console.WriteLine("CartListInv:");
            foreach (Item item in invItemList)
            {
                Console.WriteLine(item);
            }
            return true;
        }
    }
}
