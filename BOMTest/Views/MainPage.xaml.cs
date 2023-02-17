using BOMTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BOMTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = ClassLocator.Services.GetService<MainViewModel>();
        }

        private async void Receive_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await ServerReceive.ScrollToAsync(0, ServerReceive.ContentSize.Height, true);
        }
    }
}