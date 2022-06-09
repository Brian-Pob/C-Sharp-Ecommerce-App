﻿using System;
namespace Library.CIS_Proj.Models
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

		public Product()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = 0;
            IsBogo = false;
        }
		public Product Clone()
		{
			return (Product)this.MemberwiseClone();
		}

		public decimal TotalPrice { get; }

        
	}
}

