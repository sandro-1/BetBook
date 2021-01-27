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
    public partial class LoginPage : ContentPage
    {
        LoginViewModel loginVM;
        public LoginPage()
        {
            InitializeComponent();
            loginVM = new LoginViewModel();
            BindingContext = loginVM;

            //MessagingCenter.Subscribe<object, bool>(Application.Current, "Refresh", (sender, arg) =>
            //{
            //    loginVM.RefreshCommand.Execute(null);
            //    MessagingCenter.Send<object, bool>(Application.Current, "RefreshPages", true);
            //});
        }
    }
}