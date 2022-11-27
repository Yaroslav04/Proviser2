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

            string[] menu = { "Посилання", "Текст рішення" };
            string answer = await Shell.Current.DisplayActionSheet("Оберіть дію", "Cancel", null, menu);

            if (answer == menu[0])
            {
                await Browser.OpenAsync("https://reyestr.court.gov.ua/Review/" + _item.Id, BrowserLaunchMode.SystemPreferred);
            }
            else if (answer == menu[1])
            {
                await Shell.Current.DisplayAlert("Судове рішення", _item.Content, "OK");
            }

        }
    }
}

