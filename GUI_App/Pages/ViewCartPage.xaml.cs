using GUI_App.Dialogs;
using Library.GUI_App.Models;
using Library.GUI_App.Services;
using Library.Standard.CIS_Proj.Utilities;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUI_App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewCartPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private CartService _CartService { get; set; }
        public ObservableCollection<Product> Products
        {
            get
            {
                if (_CartService == null)
                {
                    _CartService = CartService.Current;
                    return new ObservableCollection<Product>();
                }
                return new ObservableCollection<Product>(CartService.Current.Products);
            }
        }
        private string SelectedCart { get; set; }
        public Product SelectedProduct { get; set; }
        public bool IsSelectedProduct { get { return SelectedProduct != null; } }
        public bool IsCartNotEmpty { get { return Products.Count != 0; } }
        public string CheckoutButtonText { get { return "Checkout (Subtotal: $" + CartService.Current.GetTotal() + ")"; } }
        public ViewCartPage()
        {
            this.InitializeComponent();
            _CartService = CartService.Current;
            DataContext = this;
            NotifyPropertyChanged("Products");
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedCart = (string)e.Parameter;
        }

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProduct = (sender as DataGrid).SelectedItem as Product;
            NotifyPropertyChanged("IsSelectedProduct");
        }


        private void DataGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == "Name")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Name ascending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Name descending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            if (e.Column.Tag.ToString() == "Id")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Id ascending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Id descending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            if (e.Column.Tag.ToString() == "Description")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Description ascending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Description descending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            if (e.Column.Tag.ToString() == "Price")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Price ascending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Price descending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            if (e.Column.Tag.ToString() == "IsBogo")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.IsBogo ascending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.IsBogo descending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            if (e.Column.Tag.ToString() == "Count")
            {
                //var pcopy = new ObservableCollection<Product>(Products);
                //for (int i = 0; i < pcopy.Count; i++)
                //{
                //    if(pcopy[i] is ProductByQuantity)
                //    {

                //    }
                //}
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {

                    dg.ItemsSource = new ObservableCollection<Product>(Products.OrderBy(p => (p is ProductByQuantity) ? ((ProductByQuantity)p).Count : ((ProductByWeight)p).Count));

                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(Products.OrderByDescending(p => (p is ProductByQuantity) ? ((ProductByQuantity)p).Count : ((ProductByWeight)p).Count));

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            if (e.Column.Tag.ToString() == "Unit")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(Products.OrderBy(p => (p is ProductByQuantity) ? ((ProductByQuantity)p).Unit : ((ProductByWeight)p).Unit));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(Products.OrderByDescending(p => (p is ProductByQuantity) ? ((ProductByQuantity)p).Unit : ((ProductByWeight)p).Unit));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
            
            if (e.Column.Tag.ToString() == "TotalPrice")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    dg.ItemsSource = new ObservableCollection<Product>(Products.OrderBy(p => (p is ProductByQuantity) ? ((ProductByQuantity)p).TotalPrice : ((ProductByWeight)p).TotalPrice));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(Products.OrderByDescending(p => (p is ProductByQuantity) ? ((ProductByQuantity)p).TotalPrice : ((ProductByWeight)p).TotalPrice));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }

            RemoveSortDirection(e);
        }

        private void RemoveSortDirection(Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e = null)
        {

            foreach (var dgColumn in dg.Columns)
            {
                if (e == null || dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(MainPage), null);
        }

        private async void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RemoveItemFromCartDialog(SelectedProduct);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                NotifyPropertyChanged("Products");
                NotifyPropertyChanged("CheckoutButtonText");
                NotifyPropertyChanged("IsCartNotEmpty");
                NotifyPropertyChanged("IsSelectedProduct");
            }
        }

        private async void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {

            // Enter fake payment info
            // Show subtotal and total
            // Ensure BOGO works
            // if checkout is successful, delete cart file
            var subtotal = CartService.Current.GetTotal();
            var taxrate = (decimal)0.075;
            var total = subtotal + (subtotal * taxrate);
            var dialog = new CheckoutDialog();
            dialog.Title = $"Checkout (Tax: ${Decimal.Round(subtotal * taxrate, 2)}, Total: ${(Decimal.Round(total,2))})";
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                //if (SelectedCart != null)
                //{
                //    if (!File.Exists(CartService.Current.persistPath + SelectedCart + ".json"))
                //        return;
                //    File.Delete((CartService.Current.persistPath + SelectedCart + ".json"));
                //}
                // delete cart file

                CartService.Current.Products.Clear();
                // delete cart from the database
                _ = new WebRequestHandler().Post($"http://localhost:5017/api/Carts/Delete", CartService.Current.CartName).Result;

                NotifyPropertyChanged("Products");
                Frame.Navigate(typeof(MainPage), "checkedout");
            }
        }

        private bool _isSearching = true;

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isSearching)
            {
                CartService.Current.SearchTerm = (SearchTextBox.Text);
                _isSearching = false;
                dg.ItemsSource = new ObservableCollection<Product>(CartService.Current.SearchResults);
                SearchBtn.Content = "Clear";
            }
            else
            {
                _isSearching = true;
                dg.ItemsSource = Products;
                SearchBtn.Content = "Search";
                SearchTextBox.Text = "";
            }

        }
    }
}
