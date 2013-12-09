using GTec.User.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GTec.User.View
{
    public sealed partial class MainPage : Page
    {
        private Controller.Control control = new Controller.Control();

        public Controller.Control Control
        {
            get { return control; }
        }

        public MainPage()
        {
            Controller.Control.ThreadsToNotify.Add(this);
            this.InitializeComponent();
        }
    }
}
