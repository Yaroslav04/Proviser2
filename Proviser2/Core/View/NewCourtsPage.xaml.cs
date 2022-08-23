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
    public partial class NewCourtsPage : ContentPage
    {
        NewCourtsViewModel newCourtsViewModel;
        public NewCourtsPage()
        {
            InitializeComponent();
            newCourtsViewModel = new NewCourtsViewModel();
            BindingContext = newCourtsViewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            newCourtsViewModel.OnAppearing();
        }
    }
}