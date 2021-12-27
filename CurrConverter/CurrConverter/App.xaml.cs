using System;
using CurrConverter.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("Poppins-Black.ttf", Alias = "PoppinsBlack")]
[assembly: ExportFont("Poppins-Italic.ttf", Alias = "PoppinsItalic")]

namespace CurrConverter
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            if (MainPage.BindingContext is MainViewModel viewModel)
            {
                //viewModel.LoadInfoPreferences();
            }
        }

        protected override void OnSleep()
        {
            if (MainPage.BindingContext is MainViewModel viewModel)
            {
                viewModel.SaveInfoPreferences();
            }
        }

        protected override void OnResume()
        {
            if (MainPage.BindingContext is MainViewModel viewModel)
            {
                viewModel.LoadInfoPreferences();
            }
        }
    }
}
