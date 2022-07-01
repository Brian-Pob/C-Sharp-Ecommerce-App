using Library.GUI_App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Library.GUI_App.Services;
using Library.GUI_App.Models;
using Library.GUI_App.Utilities;
using Windows.Globalization.NumberFormatting;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUI_App.Dialogs
{
    public sealed partial class AddToCartDialog : ContentDialog
    {

        private Product SelectedProduct;
        private double MaxCount { get; set; }
        public AddToCartDialog()
        {
            this.InitializeComponent();
        }

        public AddToCartDialog(Product selectedProduct)
        {
            this.InitializeComponent();
            SelectedProduct = selectedProduct;
            this.DataContext = SelectedProduct.Clone();

            Title = $"Adding {(this.DataContext as Product).Name} to Cart";
            if (this.DataContext is ProductByQuantity)
            {
                MaxCount = (double)(this.DataContext as ProductByQuantity).Count;
                CountText.Text = "Qty. to add:";
                UseQuantity();
            }
            else
            {
                MaxCount = (double)(this.DataContext as ProductByWeight).Weight;
                CountText.Text = "Weight (lbs.) to add:";
                UseWeight();
            }
            CountBox.Header = $"Max: {MaxCount}";
            CountBox.Maximum = MaxCount;

            /* TODO: Code for adding to cart should subtract from inventory service from within the cart service */
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (this.DataContext is ProductByQuantity)
            {
                (this.DataContext as ProductByQuantity).Quantity = (int)CountBox.Value;
                CartService.Current.AddToCart(this.DataContext as ProductByQuantity);
            }
            else
            {
                (this.DataContext as ProductByWeight).Weight = (decimal)CountBox.Value;
                CartService.Current.AddToCart(this.DataContext as ProductByWeight);
            }
            
            var test = CartService.Current.Products;
            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void CountBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if (CountBox.Value == 0)
            {
                IsPrimaryButtonEnabled = false;
            }
            else
            {
                IsPrimaryButtonEnabled = true;
            }
        }

        private void UseQuantity()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 1;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundUp;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 0;
            formatter.NumberRounder = rounder;
            CountBox.NumberFormatter = formatter;
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
        }
    }
}
