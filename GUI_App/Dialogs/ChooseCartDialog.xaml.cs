using Library.GUI_App.Models;
using Library.GUI_App.Services;
using Library.Standard.CIS_Proj.Utilities;
using Newtonsoft.Json;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUI_App.Dialogs
{
    public sealed partial class ChooseCartDialog : ContentDialog, INotifyPropertyChanged
    {
        private string persistPath
            = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Carts\\";
        private ObservableCollection<string> CartList { get; set; }
        
        private string _selectedCart;
        public string SelectedCart 
        { 
            get 
            {
                if (string.IsNullOrWhiteSpace(_selectedCart))
                    return $"cart_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
                return _selectedCart; 
            } 
            set
            {
                _selectedCart = value;
                NotifyPropertyChanged(); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ChooseCartDialog()
        {
            this.InitializeComponent();
            DataContext = this;
            Row1.Visibility = Visibility.Collapsed;
            CartList = new ObservableCollection<string>();
            
            var cartsJson = new WebRequestHandler().Get("http://localhost:5017/api/Carts").Result;
            var cartsDict = JsonConvert.DeserializeObject<Dictionary<string, List<Product>>>(cartsJson);
            foreach (var c in cartsDict) {
                CartList.Add(c.Key);
            }
            
            NotifyPropertyChanged("CartList");
            CartComboBox.ItemsSource = CartList;
            CartComboBox.SelectedIndex = 0;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (IsNewCart_cb.IsChecked == false)
            {
                CartService.Current.Load(CartList[CartComboBox.SelectedIndex]);
                //SelectedCart = CartList[CartComboBox.SelectedIndex].Replace(".json", "");
                SelectedCart = CartList[CartComboBox.SelectedIndex];
            }
            else
            {
                SelectedCart = SelectedCart_tb.Text;
            }

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // create a new cart
        }

        private void IsNewCart_Checked(object sender, RoutedEventArgs e)
        {
            if (Row0.Visibility == Visibility.Collapsed)
            {
                Row0.Visibility = Visibility.Visible;
                Row1.Visibility = Visibility.Collapsed;
            }
            else
            {
                Row0.Visibility = Visibility.Collapsed;
                Row1.Visibility = Visibility.Visible;
            }
        }
    }
}
