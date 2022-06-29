using Library.GUI_App.Models;
using Library.GUI_App.Services;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        public ViewCartPage()
        {
            this.InitializeComponent();
            _CartService = CartService.Current;
            DataContext = this;
            NotifyPropertyChanged("Products");
        }

        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
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
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Unit ascending
                                                                       select p);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;

                }
                else
                {
                    dg.ItemsSource = new ObservableCollection<Product>(from p in Products
                                                                       orderby p.Unit descending
                                                                       select p);
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
            Frame.GoBack();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
