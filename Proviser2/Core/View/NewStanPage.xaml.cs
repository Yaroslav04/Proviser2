using Proviser2.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser2.Core.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewStanPage : ContentPage
    {
        NewStanViewModel newStanViewModel;
        public NewStanPage()
        {
            InitializeComponent();
            newStanViewModel = new NewStanViewModel();
            BindingContext = newStanViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            newStanViewModel.OnAppearing();
        }
    }
}