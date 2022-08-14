using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Proviser2.Core.View;

namespace Proviser2.Core.ViewModel
{
    public class CasesListViewModel : BaseViewModel
    {
        public CasesListViewModel()
        {
            Title = "Судові справи";

            Items = new ObservableCollection<CaseClass>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<CaseClass>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
      
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
            
        }

        #region Properties

        private CaseClass selectedItem;
        public ObservableCollection<CaseClass> Items { get; }

        #endregion

        #region Commands

        public Command<CaseClass> ItemTapped { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }

        #endregion

        #region Functions

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.DataBase.GetCasesAsync();
                items = items.OrderBy(x => x.Header).ToList();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public CaseClass SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem()
        {
            await Shell.Current.GoToAsync(nameof(AddCasePage));
        }

        async void OnItemSelected(CaseClass item)
        {

            if (item == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(CasePage)}?{nameof(CaseViewModel.CaseId)}={item.Case}");
        }

        #endregion
    }
}
