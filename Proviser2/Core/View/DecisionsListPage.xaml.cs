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
    public partial class DecisionsListPage : ContentPage
    {
        DecisionsListViewModel decisionsListViewModel;
        public DecisionsListPage()
        {
            InitializeComponent();
            decisionsListViewModel = new DecisionsListViewModel();
            BindingContext = decisionsListViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            decisionsListViewModel.OnAppearing();
        }
    }
}