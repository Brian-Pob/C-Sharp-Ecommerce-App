using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Standard.CIS_Proj.Utilities;
using Newtonsoft.Json;

namespace Library.GUI_App.Models
{
    [JsonConverter(typeof(ProductJsonConverter))]
    
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
                if (IsBogo)
                {
                    return Decimal.Round((Decimal.Truncate(Weight / 2) + Weight % 2) * Price, 2);
                }

                return Decimal.Round(Weight * Price, 2);
            }
        }

        public new decimal Count
        {
            get { return Weight; }
            set { Weight = value; }
        }

        public new string Unit
        {
            get { return "lbs"; }
        }

        public ProductByWeight()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = -1;
            Weight = -1;
            IsBogo = false;
        }

        public override string ToString()
        {
           
            return $"{Id} - {Name}: {Description}. ${Price}/lb x {Weight}lbs = ${TotalPrice} {(IsBogo ? "BOGO" : "")}";
        }

        public new ProductByWeight Clone()
        {
            return (ProductByWeight)this.MemberwiseClone();
        }
    }
}
