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
    public partial class CasePage : ContentPage
    {
        public CasePage()
        {
            InitializeComponent();
            BindingContext = new CaseViewModel();
        }
    }
}