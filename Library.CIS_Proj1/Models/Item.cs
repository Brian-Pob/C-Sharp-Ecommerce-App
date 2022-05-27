using System;
namespace Library.CIS_Proj1.Models
{
	public partial class Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		private decimal price;
		public decimal Price {
			get
            {
				return price;
            }
			set
			{
				price = Decimal.Round(value, 2);
			}
		}
		public int Quantity { get; set; }
		public decimal TotalPrice
        {
			get
            {
				return Price * Quantity;
            }
        }

		public Item()
		{
			Name = string.Empty;
			Description = string.Empty;
			Price = 0;
			Quantity = 0;
		}

        public override string ToString()
        {
            return $"{Id} - {Name}: {Description}. ${Price} x {Quantity} = ${TotalPrice}";
        }

		public Item Clone()
		{
			return (Item)this.MemberwiseClone();
		}
	}
}

