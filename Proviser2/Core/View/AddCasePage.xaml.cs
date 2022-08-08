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
    public partial class AddCasePage : ContentPage
    {
        public AddCasePage()
        {
            InitializeComponent();
            BindingContext = new AddCaseViewModel();
        }
    }
}