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
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization.NumberFormatting;
using Library.GUI_App.Models;
using Library.GUI_App.Services;
using Library.GUI_App.Utilities;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUI_App.Dialogs
{
    public sealed partial class AddItemDialog : ContentDialog
    {
        private ProductType productType;
        private double Count { get; set; }
        public AddItemDialog()
        {
            this.InitializeComponent();
            this.DataContext = new Product();
            UseQuantity();
            SetPriceFormat();
        }

        
        // ADD
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dc = this.DataContext as Product;
            dc.Price = (decimal)PriceBox.Value; // Need to do this because NumberBox uses double and no way to set type as decimal
                                          // If not casted, Price will be 0.

            if(productType == ProductType.Quantity)
            {
                ProductByQuantity qdc = new ProductByQuantity();
                qdc.Name = dc.Name;
                qdc.Description = dc.Description;
                qdc.Price = dc.Price;
                qdc.Quantity = (int)CountBox.Value;
                qdc.IsBogo = (bool)(BogoCb as CheckBox).IsChecked;
                InventoryService.Current.Create(qdc.Clone());
            }
            else if(productType == ProductType.Weight)
            {
                ProductByWeight wdc = new ProductByWeight();
                wdc.Name = dc.Name;
                wdc.Description = dc.Description;
                wdc.Price = dc.Price;
                wdc.Weight = (decimal)CountBox.Value;
                wdc.IsBogo = (bool)(BogoCb as CheckBox).IsChecked;
                InventoryService.Current.Create(wdc.Clone());
            }

        }

        // CANCEL
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            var textBlock = FindName("QtyOrWtTextBlock") as TextBlock;
            var priceTextBlock = FindName("PriceTextBlock") as TextBlock;
            if (rb.Name == "QuantityRadBtn")
            {
                textBlock.Text = "Quantity";
                priceTextBlock.Text = "Price";
                productType = ProductType.Quantity;
                UseQuantity();
            }
            else if (rb.Name == "WeightRadBtn")
            {
                textBlock.Text = "Weight";
                priceTextBlock.Text = "Price per lb";
                productType = ProductType.Weight;
                UseWeight();
            }
        }

        private void BogoCbCheck(object sender, RoutedEventArgs e)
        {

        }

        private void BogoCbUncheck(object sender, RoutedEventArgs e)
        {

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

        private void SetPriceFormat()
        {
            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 0.01;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundDown;

            DecimalFormatter formatter = new DecimalFormatter();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            PriceBox.NumberFormatter = formatter;
        }
    }
}
