using BetBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BetBook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettledBetsPaidPage : ContentPage
    {
        SettledBetsPaidViewModel settledBetsPaidVM;
        public SettledBetsPaidPage()
        {
            InitializeComponent();
            settledBetsPaidVM = new SettledBetsPaidViewModel();
            BindingContext = settledBetsPaidVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            settledBetsPaidVM.RefreshCommand.Execute(null);
        }
    }
}