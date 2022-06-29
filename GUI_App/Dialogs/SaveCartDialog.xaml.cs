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
    public sealed partial class SaveCartDialog : ContentDialog
    {
        private string persistPath
            = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Carts\\";
        private ObservableCollection<string> CartList { get; set; }
        private string _selectedCart;

        private string SelectedCart
        {
            get
            {
                if (_selectedCart == null)
                {
                    return $"cart_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.json";
                }
                else
                {
                    return _selectedCart + ".json";
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public SaveCartDialog(string SelectedCart)
        {
            this.InitializeComponent();
            // Count number of files in persistPath

            if (SelectedCart == null)
            {
                Row0.Visibility = Visibility.Collapsed; // Hide Cart selector
                Row1.Visibility = Visibility.Visible;   // Show Cart name textbox
            }
            else
            {
                _selectedCart = SelectedCart;
                Row1.Visibility = Visibility.Collapsed;
                SelectedCartBlock.Text = SelectedCartBlock.Text + SelectedCart + ".json";
                PrimaryButtonText = "Confirm";
            }
        }

       

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CartService.Current.Save(SelectedCart);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        //private void CartComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            
        //    _selectedCart = CartList[CartComboBox.SelectedIndex];
        //}

        private void CartNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _selectedCart = CartNameTextBox.Text;
        }
    }
}
