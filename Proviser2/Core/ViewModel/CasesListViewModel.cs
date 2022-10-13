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

            SearchCommand = new Command(Search);

            ItemTapped = new Command<CaseClass>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
      
        }

        private void Search()
        {
            IsBusy = true;
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
            
        }

        #region Properties

        private CaseClass selectedItem;
        public ObservableCollection<CaseClass> Items { get; }

        private string searchText = null;
        public string SearchText
        {
            get => searchText;
            set
            {
                SetProperty(ref searchText, value);
            }
        }

        #endregion

        #region Commands

        public Command<CaseClass> ItemTapped { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }

        public Command SearchCommand { get; }

        #endregion

        #region Functions

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                if (String.IsNullOrWhiteSpace(SearchText))
                {
                    var items = await App.DataBase.GetCasesAsync();
                    var subitems = items.Where(x => String.IsNullOrEmpty(x.Header)).ToList();
                    items = items.OrderBy(x => x.Header).Where(x => !String.IsNullOrEmpty(x.Header)).ToList();
                    if (subitems.Count > 0)
                    {
                        items.AddRange(subitems);
                    }
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    var i = await App.DataBase.GetCasesAsync();
                    var items = i.Where(x => x.Header.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (items.Count > 0)
                    {
                        var subitems = items.Where(x => String.IsNullOrEmpty(x.Header)).ToList();
                        items = items.OrderBy(x => x.Header).Where(x => !String.IsNullOrEmpty(x.Header)).ToList();
                        if (subitems.Count > 0)
                        {
                            items.AddRange(subitems);
                        }
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }
                    }                
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
