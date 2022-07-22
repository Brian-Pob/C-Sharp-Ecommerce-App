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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GUI_App.Dialogs
{
    public sealed partial class NameCartDialog : ContentDialog
    {
        private string _cartName;
        public string CartName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_cartName))
                    return $"cart_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}";

                return _cartName;
            }

            set
            {
                _cartName = value;
            }
        }
        public NameCartDialog()
        {
            this.InitializeComponent();
            DataContext = this;

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
