using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrConverter.ViewModels;
using Xamarin.Forms;

namespace CurrConverter
{
    public partial class MainPage : ContentPage
    {
        public Frame FirstTextFrameProp => FirstTextFrame;
        public Frame SecondTextFrameProp => SecondTextFrame;
        public MainPage()
        {
            BindingContext = new MainViewModel(this);
            InitializeComponent();
        }
    }
}
