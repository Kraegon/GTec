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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GTec.User.View
{
    public sealed partial class InfoBox : UserControl
    {
        public InfoBox()
        {
            this.InitializeComponent();
        }

        private void CloseInfobox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Infobox.Visibility = Visibility.Collapsed;
        }

        public void AddData(MainPage.InfoBoxData ibd)
        {
            TitleBlock.Text = ibd.Title;
            DescriptionBlock.Text = ibd.Description;
            BitmapImage image = new BitmapImage(new Uri(ibd.ImagePath, UriKind.Absolute));
            ImageBlock.Source = image;
        }
    }
}
