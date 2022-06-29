using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GUI_App.Models
{
    public partial class ProductByQuantity : Product
    {
        public int Quantity { get; set; }
        public new decimal TotalPrice 
        { 
            get
            {
                if (IsBogo)
                {
                    return (Quantity / 2 + Quantity % 2) * Price;
                }
                return Quantity * Price;
            }
        }

        public ProductByQuantity()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = -1;
            Quantity = -1;
            IsBogo = false;
        }

        public override string ToString()
        {
            return $"{Id} - {Name}: {Description}. ${Price} x {Quantity} = ${TotalPrice} {(IsBogo ? "BOGO" : "")}";
        }

        public new decimal Count
        {
            get { return Quantity; }
            set { Quantity = (int)decimal.Round(value); }
        }
        
        public new string Unit
        {
            get { return "pcs."; }
        }
        public new ProductByQuantity Clone()
        {
            return (ProductByQuantity)this.MemberwiseClone();
        }
    }
}
