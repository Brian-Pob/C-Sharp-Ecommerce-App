using System;
namespace Library.GUI_App.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
        public bool IsBogo { get; set; }
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
		public Double DoublePrice
        {
            get
            {
				return (double)price;
            }
            set
            {
				Price = (decimal)value;
            }
        }

		public decimal Count { get; set; }
        public string Unit { get; set; }
        public Product()
        {
			Id = -1;
            Name = string.Empty;
            Description = string.Empty;
            //Price = 0;
            IsBogo = false;
        }
		public Product Clone()
		{
			return (Product)this.MemberwiseClone();
		}

		public virtual decimal TotalPrice { get { return -1; } }

        
	}
}

