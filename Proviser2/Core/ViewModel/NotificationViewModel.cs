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

namespace Proviser2.Core.ViewModel
{
    public class NotificationViewModel : BaseViewModel
    {

        public NotificationViewModel() 
        {
            Title = "Нові надходження";
            Items = new ObservableCollection<NotificationClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<NotificationClass>(OnItemSelected);

        }
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        #region Properties

        private NotificationClass _selectedItem;
        public NotificationClass SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ObservableCollection<NotificationClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<NotificationClass> ItemTapped { get; }
        public Command AddEventCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                List<NotificationClass> list = new List<NotificationClass>();
                var courts = await NotificationManager.GetCourts();
                if (courts.Count > 0)
                {
                    foreach (var item in courts)
                    {
                        list.Add(item);
                    }
                }

                var decisions = await NotificationManager.GetDecisions();
                if (decisions.Count > 0)
                {
                    foreach (var item in decisions)
                    {
                        list.Add(item);
                    }
                }

                Items.Clear();

                if (list.Count > 0) 
                {
                    list = list.OrderByDescending(x => x.Date).ToList();
                    foreach (var item in list)
                    {
                        Items.Add(item);
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

        async void OnItemSelected(NotificationClass item)
        {
            if (item == null)
                return;

            if (item.Type == "Судове засідання")
            {
                await Shell.Current.GoToAsync($"{nameof(CasePage)}?{nameof(CaseViewModel.CaseId)}={item.Link}");

            }

            if (item.Type == "Судове рішення")
            {
                await Browser.OpenAsync(item.Link, BrowserLaunchMode.SystemPreferred);

            }

        }
    }
}
