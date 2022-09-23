using System;
using Microsoft.Extensions.DependencyInjection;
using BOMTest.ViewModels;

namespace BOMTest
{
    public class ClassLocator
    {
        public static IServiceProvider Services { get; private set; }
        public static IServiceCollection ServicesCollection = new ServiceCollection();

        public ClassLocator()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            ServicesCollection.AddSingleton<MainViewModel>();
            ServicesCollection.AddSingleton<LoginViewModel>();
            
            return ServicesCollection.BuildServiceProvider();
        }

        public static void InitializeServiceProvider()
        {
            Services = ConfigureServices();
        }

        public static void UpdateServiceProvider()
        {
            Services = ServicesCollection.BuildServiceProvider();
        }
    }
}
