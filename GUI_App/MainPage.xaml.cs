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
        private InventoryService _InventoryService;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Product SelectedProduct { get; set; }
        public List<Product> Products
        {
            get
            {
                if(_InventoryService == null)
                {
                    return new List<Product>();
                }
                return (InventoryService.Current.Products);
            }
        }
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            ShowLoginDialog();
            _InventoryService = InventoryService.Current;
            

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
            LoginTypeText.Text = _loginType.ToString();
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
