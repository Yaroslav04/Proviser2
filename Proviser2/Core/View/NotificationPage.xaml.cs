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
    public partial class NotificationPage : ContentPage
    {
        NotificationViewModel notificationViewModel;
        public NotificationPage()
        {
            InitializeComponent();
            notificationViewModel = new NotificationViewModel();
            BindingContext = notificationViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            notificationViewModel.OnAppearing();
        }
    }
}