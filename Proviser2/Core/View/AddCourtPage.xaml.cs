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
    public partial class AddCourtPage : ContentPage
    {
        AddCourtViewModel addCourtViewModel;
        public AddCourtPage()
        {
            InitializeComponent();
            addCourtViewModel = new AddCourtViewModel();
            BindingContext = addCourtViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            addCourtViewModel.OnAppearing();
        }
    }
}