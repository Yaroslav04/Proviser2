using Proviser2.Core.Model;
using Proviser2.Core.Servises.UserInterfaseConverters;
using Proviser2.Core.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class EventsListViewModel : BaseViewModel
    {

        public EventsListViewModel()
        {
            Title = "Журнал подій";
            Items = new ObservableCollection<EventSoketClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<EventSoketClass>(OnItemSelected);
            AddEventCommand = new Command(Add);
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

        private EventSoketClass _selectedItem;
        public EventSoketClass SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ObservableCollection<EventSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<EventSoketClass> ItemTapped { get; }
        public Command AddEventCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                string header = await App.DataBase.GetHeaderAsync(CaseId);
                var items = await App.DataBase.GetEventsAsync(CaseId);
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        EventSoketClass eventSoketClass = new EventSoketClass(item);
                        eventSoketClass.DateSoket = item.Date.ToShortDateString();
                        eventSoketClass.Header = header;
                        Items.Add(eventSoketClass);
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
        async void OnItemSelected(EventSoketClass item)
        {
            if (item == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(EventPage)}?{nameof(EventViewModel.EventId)}={item.N}\t{CaseId}");
        }

        private async void Add()
        {
            await Shell.Current.GoToAsync($"{nameof(EventPage)}?{nameof(EventViewModel.EventId)}=new\t{CaseId}");
        }
    }
}
