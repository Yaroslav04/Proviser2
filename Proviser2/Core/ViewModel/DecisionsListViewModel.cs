using Proviser2.Core.Model;
using Proviser2.Core.Servises;
using Proviser2.Core.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class DecisionsListViewModel : BaseViewModel
    {

        public DecisionsListViewModel()
        {
            Title = "Судові рішення";
            Items = new ObservableCollection<DecisionSoketClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<DecisionSoketClass>(OnItemSelected);
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        #region Properties

        private string caseId;
        public string CaseId
        {
            get => caseId;
            set
            {
                SetProperty(ref caseId, value);
            }
        }

        private DecisionSoketClass _selectedItem;
        public DecisionSoketClass SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ObservableCollection<DecisionSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<DecisionSoketClass> ItemTapped { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();

                var items = await App.DataBase.GetDecisionsAsync(CaseId);
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        Items.Add(await ClassConverter.ConvertDecisionClassToSoket(item));
                    }
                }
            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
        async void OnItemSelected(DecisionSoketClass _item)
        {
            if (_item == null)
                return;
            await Browser.OpenAsync("https://reyestr.court.gov.ua/Review/" + _item.Id, BrowserLaunchMode.SystemPreferred);
        }

    }
}
