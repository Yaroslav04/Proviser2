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
        public ObservableCollection<CourtSoketClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<CourtSoketClass> ItemTapped { get; }

        #endregion

        #region Functions
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.DataBase.GetCourtsHearingOrderingByDateAsync();
                if (items.Count == 0)
                {
                    return;
                }
                else
                {
                    foreach (var item in items)
                    {
                        try
                        {
                            var subCase = await App.DataBase.GetCasesByCaseAsync(item.Case);
                            CourtSoketClass courtSoketClass = new CourtSoketClass(item);
                            courtSoketClass.PrisonDate = TextManager.GetBeautifyPrisonDate(subCase.PrisonDate);
                            courtSoketClass.Header = subCase.Header;
                            courtSoketClass.Note = subCase.Note;
                            courtSoketClass.PrisonDate = TextManager.GetBeautifyPrisonDate(subCase.PrisonDate);
                            Items.Add(courtSoketClass);
                        }
                        catch
                        {

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

        async void OnItemSelected(CourtSoketClass item)
        {

            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(CasePage)}?{nameof(CaseViewModel.CaseId)}={item.Case}");
        }

        #endregion
    }
}
