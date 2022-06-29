using Library.GUI_App.Services;
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
        public string SelectedCart { get { return _selectedCart; } set { _selectedCart = value; NotifyPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ChooseCartDialog()
        {
            this.InitializeComponent();
            DataContext = this;
            // Count number of files in persistPath
            var files = Directory.GetFiles(persistPath);
            CartList = new ObservableCollection<string>();
            foreach (var file in files)
            {
                CartList.Add(Path.GetFileName(file));
            }
            NotifyPropertyChanged("CartList");
            CartComboBox.ItemsSource = CartList;
            CartComboBox.SelectedIndex = 0;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CartService.Current.Load(CartList[CartComboBox.SelectedIndex]);
            SelectedCart = CartList[CartComboBox.SelectedIndex].Replace(".json", "");
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
