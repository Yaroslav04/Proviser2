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
    public partial class EventsListPage : ContentPage
    {
        EventsListViewModel eventsListViewModel;
        public EventsListPage()
        {
            InitializeComponent();
            eventsListViewModel = new EventsListViewModel();
            BindingContext = eventsListViewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            eventsListViewModel.OnAppearing();
        }
    }
}