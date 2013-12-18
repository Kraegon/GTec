using GTec.User.Controller;
using System;
using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace GTec.User.View
{
    public class BaseClassForBindableProperties : INotifyPropertyChanged
    {
        /// <summary>
        /// Gives the UIThread a sign to update.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected async void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null) return;
            foreach (UIElement uiElement in Control.GetInstance().ThreadsToNotify)
            {
                await uiElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        handler(this, new PropertyChangedEventArgs(propertyName));
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                });
            }
        }
    }
}
