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
using System.Linq;
using static SQLite.SQLite3;

namespace Proviser2.Core.ViewModel
{
    public class NewCourtsViewModel : BaseViewModel
    {

        public NewCourtsViewModel() 
        {
            Title = "Нові засідання";
            Items = new ObservableCollection<CourtSoketClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<CourtSoketClass>(OnItemSelected);

        }
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        #region Properties

        private CourtSoketClass _selectedItem;
        public CourtSoketClass SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        public ObservableCollection<CourtSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<CourtSoketClass> ItemTapped { get; }
        public Command AddEventCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                var courts = await App.DataBase.GetLastCourtsAsync();
                if (courts.Count == 0)
                {
                    return;
                }
                else
                {
                    Items.Clear();
                    foreach (var item in courts)
                    {
                        Items.Add(await ClassConverter.ConvertCourtClassToSoket(item));
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

        async void OnItemSelected(CourtSoketClass item)
        {
            if (item == null)
                return;

                await Shell.Current.GoToAsync($"{nameof(CasePage)}?{nameof(CaseViewModel.CaseId)}={item.Case}");

        }
    }
}
