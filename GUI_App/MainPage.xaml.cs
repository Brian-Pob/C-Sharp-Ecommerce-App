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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Library.GUI_App.Models;
using Library.GUI_App.Services;
using Library.GUI_App.Utilities;
using GUI_App.Dialogs;
using Microsoft.Toolkit.Uwp.UI.Controls;
using GUI_App.Pages;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GUI_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public LoginType _loginType { get; set; }
        public string Query { get; set; }
        private InventoryService _InventoryService { get; set; }
        private CartService _CartService { get; set; }
        private string SelectedCart;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Product SelectedProduct { get; set; }
        public ObservableCollection<Product> Products
        {
            get
            {
                if(_InventoryService == null)
                {
                    return new ObservableCollection<Product>();
                }
                return new ObservableCollection<Product>(InventoryService.Current.Products);
            }
        }
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            //ShowLoginDialog();
            _loginType = LoginType.Employee;
            _InventoryService = InventoryService.Current;
            _CartService = CartService.Current;
            
            _InventoryService.CartService = _CartService;
            _CartService.InventoryService = _InventoryService;

            InventoryService.Current.Load();

            // if cartPersistPath directory does not exist, create it

            if (!Directory.Exists(cartPersistPath))
            {
                Directory.CreateDirectory(cartPersistPath);
            }

            ShowChooseCartDialog();
            NotifyPropertyChanged("Products");
            var test = Products;

            AddToCartBtn.IsEnabled = false;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            if ((string)e.Parameter == "checkedout")
            {
                InventoryService.Current.Save();
                ShowChooseCartDialog();
            }

            NotifyPropertyChanged("Products");
        }


        private string cartPersistPath
            = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Carts\\";        
        private async void ShowChooseCartDialog()
        {
            

            try
            {
                var files = Directory.GetFiles(cartPersistPath);
                if (files.Length == 0)
                    return;
            }
            catch (Exception)
            {
                return;
            }
            

            var dialog = new ChooseCartDialog();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                SelectedCart = dialog.SelectedCart;
            }
            else
            {
                SelectedCart = null;
            }
            

        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            
            var diag = new AddItemDialog();
            await diag.ShowAsync();
            //NotifyPropertyChanged("Products");
            dg.ItemsSource = Products;
            RemoveSortDirection();
            
            //var page = new SecondaryPage();
            //Frame.Navigate(typeof(SecondaryPage));

        }

        private async void Add_To_Cart_Click(object sender, RoutedEventArgs e)
        {
            var diag = new AddToCartDialog(SelectedProduct);
            await diag.ShowAsync();
            //NotifyPropertyChanged("Products");
            dg.ItemsSource = Products;
            RemoveSortDirection();
        }

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg.SelectedItem != null)
            {
                //SelectedProduct = (Product)dg.SelectedItem;
                AddToCartBtn.IsEnabled = true;
            }
            else
            {
                AddToCartBtn.IsEnabled = false;
            }
        }


        private void DataGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            if(e.Column.Tag.ToString() == "Name")
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

            if(e.Column.Tag.ToString() == "Id")
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

            if(e.Column.Tag.ToString() == "Description")
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

        private async void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveCartDialog(SelectedCart);
            var result = await dialog.ShowAsync();


            if (result == ContentDialogResult.Primary)
            {
                if (SelectedCart == null)
                {
                    // Set SelectedCart to most recently created cart file
                    SelectedCart = dialog.SelectedCart;
                }
                InventoryService.Current.Save();
            }
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_loginType == LoginType.Employee)
            {
                _loginType = LoginType.Customer;
                AddProductBtn.Visibility = Visibility.Collapsed;
                dg.IsReadOnly = true;
            }
            else
            {
                _loginType = LoginType.Employee;
                AddProductBtn.Visibility = Visibility.Visible;
                dg.IsReadOnly = false;
            }

            NotifyPropertyChanged("_loginType");
        }

        private void ViewCartBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ViewCartPage), SelectedCart);
        }

        private void dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
        }

        private bool _isSearching = true;
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isSearching)
            {
                InventoryService.Current.SearchTerm = (SearchTextBox.Text);
                _isSearching = false;
                dg.ItemsSource = new ObservableCollection<Product>(InventoryService.Current.SearchResults);
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
