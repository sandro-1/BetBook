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
    public partial class CreateAccountPage : ContentPage
    {
        CreateAccountViewModel createAccountVM;
        public CreateAccountPage()
        {
            InitializeComponent();
            createAccountVM = new CreateAccountViewModel();
            BindingContext = createAccountVM;
        }
    }
}