using Proviser2.Core.Model;
using Proviser2.Core.Servises;
using Proviser2.Core.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class StanListViewModel : BaseViewModel
    {
        public StanListViewModel()
        {
            Title = "Стан";
            Items = new ObservableCollection<StanSoketClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            //ItemTapped = new Command<StanSoketClass>(OnItemSelected);
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

        private StanSoketClass _selectedItem;
        public StanSoketClass SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                //OnItemSelected(value);
            }
        }

        public ObservableCollection<StanSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        //public Command<StanSoketClass> ItemTapped { get; }
        //public Command AddEventCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                var stans = await App.DataBase.GetStansByCaseAsync(CaseId);
                if (stans.Count == 0)
                {
                    return;
                }
                else
                {
                    Items.Clear();
                    foreach (var item in stans)
                    {
                        Items.Add(await ClassConverter.ConvertStanClassToSoket(item));
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

        //async void OnItemSelected(StanSoketClass item)
        //{
        //    if (item == null)
        //        return;

        //    await Shell.Current.GoToAsync($"{nameof(CasePage)}?{nameof(CaseViewModel.CaseId)}={item.Case}");

        //}


    }
}
