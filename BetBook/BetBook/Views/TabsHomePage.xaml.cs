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
    public partial class TabsHomePage : TabbedPage
    {
        TabsHomeViewModel tabsVM;
        public TabsHomePage()
        {
            InitializeComponent();
            tabsVM = new TabsHomeViewModel();
            BindingContext = tabsVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            tabsVM.RefreshCommand.Execute(null);
        }
    }
}