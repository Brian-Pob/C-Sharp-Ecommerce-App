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

        public decimal TotalPrice
        {
            get
            {
                return Decimal.Round(Weight * Price, 2);
            }
        }
    }
}
