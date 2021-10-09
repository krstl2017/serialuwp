using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Runtime.CompilerServices;

namespace SerialApp.Models
{
    public class BaseDependencyObject : DependencyObject, INotifyPropertyChanged
    {
        //NOTE: added inheritance from dependencyobject to support project 2
        protected async void SetValueOnUI(DependencyProperty dp, object value)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {

                SetValue(dp, value);
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void UpdateProperty(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }




        protected async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await Windows.ApplicationModel.Core.CoreApplication
                                   .MainView.CoreWindow.Dispatcher
                                   .RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                                   {
                                       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                                   });
        }

        protected static async void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication
                       .MainView.CoreWindow.Dispatcher
                       .RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                       {

                           d.SetValue(e.Property, e.NewValue);

                       });
        }

    }
}
