using BOMTest.Services;
using BOMTest.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BOMTest
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            ClassLocator.InitializeServiceProvider();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
