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
    public class NewDecisionsViewModel : BaseViewModel
    {
        public NewDecisionsViewModel()
        {
            Title = "Нові рішення";
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
        public Command AddEventCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                var decisions = await App.DataBase.GetLastDecisionsAsync();
                if (decisions.Count == 0)
                {
                    return;
                }
                else
                {
                    Items.Clear();
                    foreach (var item in decisions)
                    {


                        DecisionSoketClass decisionSoketClass = new DecisionSoketClass(item);
                        decisionSoketClass.Header = await App.DataBase.GetHeaderAsync(item.Case);
                        decisionSoketClass.DecisionDateSoket = item.DecisionDate.ToShortDateString();
                        decisionSoketClass.LegalDateSoket = TextManager.GetBeautifyLegalDate(item.LegalDate);
                        decisionSoketClass.CategorySoket = TextManager.GetBeautifyDecisionCategory(item.Category);
                        Items.Add(decisionSoketClass);
                        Items.Add(decisionSoketClass);
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

        async void OnItemSelected(DecisionSoketClass item)
        {
            if (item == null)
                return;

            await Browser.OpenAsync(item.URL, BrowserLaunchMode.SystemPreferred);

        }
    }
}

