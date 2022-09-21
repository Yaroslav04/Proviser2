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
    public partial class StanListPage : ContentPage
    {
        StanListViewModel stanListViewModel;
        public StanListPage()
        {
            InitializeComponent();
            stanListViewModel = new StanListViewModel();
            BindingContext = stanListViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            stanListViewModel.OnAppearing();
        }
    }
}