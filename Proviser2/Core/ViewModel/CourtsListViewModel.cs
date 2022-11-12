using Proviser2.Core.Model;
using Proviser2.Core.Servises;
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
    public class CourtsListViewModel : BaseViewModel
    {

        public CourtsListViewModel()
        {
            Title = "Засідання " + DateTime.Now.ToShortDateString();
            Items = new ObservableCollection<CourtSoketClass>();      
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<CourtSoketClass>(OnItemSelected);
            SearchCommand = new Command(Search);
            SendCommand = new Command(Send);
        }

        private async void Send()
        {
            bool answer = await Shell.Current.DisplayAlert("Відправка на пошту", $"Відправити засідання на пошту", "Так", "Ні");
            if (answer)
            {
                await Task.Run(() => MailSender.SendCourtHearings());
                await Shell.Current.DisplayAlert("Відправка на пошту", "Засідання відправлено", "OK");
            }
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

        private CourtSoketClass _selectedItem;
        public CourtSoketClass SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private string searchText = null;
        public string SearchText
        {
            get => searchText;
            set
            {
                SetProperty(ref searchText, value);
            }
        }

        public ObservableCollection<CourtSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command SearchCommand { get; }
        public Command SendCommand { get; }
        public Command<CourtSoketClass> ItemTapped { get; }

        #endregion

        #region Functions

        async Task<List<CourtSoketClass>> GetCourtHearings()
        {
            List<CourtSoketClass> courtHearings = new List<CourtSoketClass>();
            var items = await App.DataBase.GetCourtsHearingOrderingByDateAsync();
            if (items.Count == 0)
            {
                return courtHearings;
            }
            else
            {
                foreach (var item in items)
                {
                    courtHearings.Add(await ClassConverter.ConvertCourtClassToSoket(item));
                }
                return courtHearings;
            }
        }
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                if (String.IsNullOrWhiteSpace(SearchText))
                {
                    try
                    {
                        foreach (var item in await GetCourtHearings())
                        {
                            Items.Add(item);
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        foreach (var item in await GetCourtHearings())
                        {
                            if (item.Header.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                            {
                                Items.Add(item);
                            }
                        }
                    }
                    catch
                    {
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

            #endregion
        }
    }
