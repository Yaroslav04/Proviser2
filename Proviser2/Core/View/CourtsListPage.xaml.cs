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
    public partial class CourtsListPage : ContentPage
    {
        CourtsListViewModel courtsListViewModel;
        public CourtsListPage()
        {
            InitializeComponent();
            courtsListViewModel = new CourtsListViewModel();
            BindingContext = courtsListViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            courtsListViewModel.OnAppearing();
        }
    }
}