using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.CIS_Proj.Models
{
    public partial class ProductByWeight : Product
    {
        private decimal weight;
        public decimal Weight 
        { 
            get
            {
                return weight;
            }

            set
            {
                weight = value;
            }
        }

        public new decimal TotalPrice
        {
            get
            {
                return Decimal.Round(Weight * Price, 2);
            }
        }

        public ProductByWeight()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = 0;
            Weight = 0;
            isBogo = false;
        }

        public override string ToString()
        {
           
            return $"{Id} - {Name}: {Description}. ${Price}/lb x {Weight}lbs = ${TotalPrice} {(isBogo ? "BOGO" : "")}";
        }

        public new ProductByWeight Clone()
        {
            return (ProductByWeight)this.MemberwiseClone();
        }
    }
}
