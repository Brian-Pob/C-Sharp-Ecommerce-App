using System;
namespace Library.CIS_Proj.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
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

		public Product Clone()
		{
			return (Product)this.MemberwiseClone();
		}

		public decimal TotalPrice { get; }
	}
}

