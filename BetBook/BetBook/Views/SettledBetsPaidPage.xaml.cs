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

            MessagingCenter.Subscribe<object, bool>(Application.Current, "RefreshPages", (sender, arg) =>
            {
                settledBetsPaidVM.RefreshCommand.Execute(null);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            settledBetsPaidVM.RefreshCommand.Execute(null);
        }
    }
}