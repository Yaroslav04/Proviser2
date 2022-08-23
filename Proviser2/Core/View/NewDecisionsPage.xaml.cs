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
    public partial class NewDecisionsPage : ContentPage
    {
        NewDecisionsViewModel newDecisionsViewModel;
        public NewDecisionsPage()
        {
            InitializeComponent();
            newDecisionsViewModel = new NewDecisionsViewModel();
            BindingContext = newDecisionsViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            newDecisionsViewModel.OnAppearing();
        }
    }
}