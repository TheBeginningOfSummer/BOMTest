using BOMTest.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace BOMTest.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}