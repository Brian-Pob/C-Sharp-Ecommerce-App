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
using GUI_App.Dialogs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Library.GUI_App.Models;
using Library.GUI_App.Services;
using Library.GUI_App.Utilities;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GUI_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LoginType _loginType { get; set; }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private InventoryService _InventoryService;
        public ObservableCollection<Product> Products
        {
            get
            {
                if(_InventoryService == null)
                {
                    return new ObservableCollection<Product>();
                }
                return new ObservableCollection<Product>(_InventoryService.Products);
            }
        }
        public Product SelectedProduct { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            ShowLoginDialog();

            _InventoryService = InventoryService.Current;
            var test = new ProductByQuantity();
            test.Name = "Test product";
            test.Description = "test prod";
            test.Price = (decimal)9.99;
            test.Quantity = 3;
            _InventoryService.Create(test);
            NotifyPropertyChanged("Products");

        }


        private async void ShowLoginDialog()
        {
            var dialog = new LoginDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _loginType = LoginType.Employee;
            }
            else
            {
                _loginType = LoginType.Customer;
            }
            var logintypetext = this.FindName("LoginTypeText") as TextBlock;
            logintypetext.Text = _loginType.ToString();
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            
            var diag = new AddItemDialog();
            await diag.ShowAsync();
            NotifyPropertyChanged("Products");

            //var page = new SecondaryPage();
            //Frame.Navigate(typeof(SecondaryPage));

        }
    }

}
