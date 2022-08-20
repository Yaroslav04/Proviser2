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
    public partial class CourtsFromCaseListPage : ContentPage
    {
        CourtsFromCaseListViewModel courtsFromCaseListViewModel;
        public CourtsFromCaseListPage()
        {
            InitializeComponent();
            courtsFromCaseListViewModel = new CourtsFromCaseListViewModel();
            BindingContext = courtsFromCaseListViewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            courtsFromCaseListViewModel.OnAppearing();
        }
    }
}