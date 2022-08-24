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
using System.Linq;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class CourtsFromCaseListViewModel : BaseViewModel
    {
        public CourtsFromCaseListViewModel()
        {
            Title = "Засідання";
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

        private string caseId;
        public string CaseId
        {
            get => caseId;
            set
            {
                SetProperty(ref caseId, value);
            }
        }

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

        public ObservableCollection<CourtSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<CourtSoketClass> ItemTapped { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();    
                var items = await App.DataBase.GetCourtsAsync(CaseId);
                if (items.Count > 0)
                {
                    items = items.OrderByDescending(x => x.Date).ToList();
                    foreach (var item in items)
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

            if (item.Origin == "local")
            {
                Debug.WriteLine(item.N);
                var result = await Shell.Current.DisplayActionSheet($"Видалити {item.Date}?", destruction: "OK", cancel: "Відміна");
                if (result == "OK")
                {
                    await App.DataBase.DeleteCourtAsync(await App.DataBase.GetCourtAsync(item.N));
                }           
            }
        }
    }
}
