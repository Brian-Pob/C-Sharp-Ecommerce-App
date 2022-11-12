using Library.GUI_App.Models;
using Library.GUI_App.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUI_App.Dialogs
{
    public sealed partial class RemoveItemFromCartDialog : ContentDialog
    {
        public decimal MaxCount
        {
            get {
                if (DataContext is ProductByWeight)
                {
                    return (DataContext as ProductByWeight).Weight;
                }
                else
                {
                    return (DataContext as ProductByQuantity).Quantity;
                }
            }
        }
        
        public RemoveItemFromCartDialog(Product product)
        {
            this.InitializeComponent();
            this.DataContext = product;
            
            Title = "Remove " + product.Name + " from cart?";
            if (product is ProductByQuantity)
            {
                CountText.Text = "Qty to remove:";
                UseQuantity();
            }
            else
            {
                CountText.Text = "Weight to remove:";
                UseWeight();
            }
            CountBox.Header = "Max: " + MaxCount;
            CountBox.Maximum = (double)MaxCount;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var product = (DataContext as Product).Clone();
            if (product is ProductByQuantity)
            {
                (product as ProductByQuantity).Quantity = (int)CountBox.Value;
            }
            else
            {
                (product as ProductByWeight).Weight = (decimal)CountBox.Value;
            }
            CartService.Current.Delete(product);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void UseQuantity()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 1;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundDown;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 0;
            formatter.NumberRounder = rounder;
            CountBox.NumberFormatter = formatter;
            CountBox.Minimum = 1;
            // Default values:
            // CountBox.SmallChange = 1
            // CountBox.LargeChange = 5
            
        }

        private void UseWeight()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 0.01;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundDown;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            CountBox.NumberFormatter = formatter;
            CountBox.Minimum = 0.01;
        }

        private void CountBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if (DataContext == null)
                return;

            var debug_cbval = CountBox.Value;
            if (double.IsNaN(CountBox.Value))
            {
                CountBox.Value = 1;
            }
            
            if (DataContext is ProductByWeight productByWeight)
            {
                if ((decimal)CountBox.Value == productByWeight.Weight)
                {
                    PrimaryButtonText = "Remove All";
                }
                else
                {
                    PrimaryButtonText = "Remove";
                }
            }
            else
            {
                if ((int)CountBox.Value == (DataContext as ProductByQuantity).Quantity)
                {
                    PrimaryButtonText = "Remove All";
                }
                else
                {
                    PrimaryButtonText = "Remove";
                }
            }
            
        }
    }
}
