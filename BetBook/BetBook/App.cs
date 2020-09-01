using BetBook.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BetBook
{
    public partial class App : Application
    {
        public App()
        {
            MainPage = new LoginPage();
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
