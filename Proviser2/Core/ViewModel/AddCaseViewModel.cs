using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Proviser2.Core.Servises;

namespace Proviser2.Core.ViewModel
{
    public class AddCaseViewModel : BaseViewModel
    {
        public AddCaseViewModel()
        {
            Title = "Додати справу";
            Items = new ObservableCollection<CourtClass>();
            SaveCommand = new Command(Save);
            SearchCommand = new Command(Search);
            ClearCommand = new Command(Clear);
        }

        #region Properties

        public ObservableCollection<CourtClass> Items { get; }

        private CourtClass selectedItem;
        public CourtClass SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                OnItemSelected(value);
            }
        }

        private string caseSearchPanel = null;
        public string CaseSearchPanel
        {
            get => caseSearchPanel;
            set
            {
                SetProperty(ref caseSearchPanel, value);
            }
        }

        private string headerEditPanel = null;
        public string HeaderEditPanel
        {
            get => headerEditPanel;
            set
            {
                SetProperty(ref headerEditPanel, value);
            }
        }

        private string littigansSerachPanel = null;
        public string LittigansSerachPanel
        {
            get => littigansSerachPanel;
            set
            {
                SetProperty(ref littigansSerachPanel, value);
            }
        }


        #endregion

        #region Commands
        public Command SaveCommand { get; }
        public Command SearchCommand { get; }
        public Command ClearCommand { get; }

        #endregion

        void OnItemSelected(CourtClass item)
        {
            if (item != null)
            {
                CaseSearchPanel = item.Case;
                HeaderEditPanel = TextManager.Header(item.Littigans);
            }

        }

        private async void Search()
        {
            Items.Clear();

            if (!String.IsNullOrWhiteSpace(CaseSearchPanel))
            {
                var result = await App.DataBase.GetCourtsAsync(CaseSearchPanel);
                if (result.Count > 0)
                {
                    var last = result.Last();
                    Items.Add(last);
                }
                else
                {
                    return;
                }

            }

            if (!String.IsNullOrWhiteSpace(LittigansSerachPanel))
            {
                if (LittigansSerachPanel.Length > 2)
                {
                    var result = await App.DataBase.GetCourtsByLittigansAsync(LittigansSerachPanel);
                    result = result.OrderByDescending(x => x.Date).Take(250).ToList();
                    List<string> cases = new List<string>();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            cases.Add(item.Case);
                        }

                        result = new List<CourtClass>();
                        cases = cases.Distinct().ToList();
                        foreach (var c in cases)
                        {
                            var subresult = await App.DataBase.GetCourtsAsync(c);
                            if (subresult.Count > 0)
                            {

                                var last = subresult.Last();
                                Items.Add(last);
                            }
                        }
                    }
                }
                return;
            }
        }

        private async void Save()
        {
            if (selectedItem != null)
            {
                CaseClass caseClass = new CaseClass();
                caseClass.Case = selectedItem.Case;
                caseClass.Header = HeaderEditPanel;
                caseClass.PrisonDate = DateTime.MinValue;
                try
                {
                    await App.DataBase.SaveCasesAsync(caseClass);
                }
                catch
                {

                }
                finally
                {
                    Clear();
                }
            }
            else if (Items.Count == 0)
            {
                CaseClass caseClass = new CaseClass();
                caseClass.Case = CaseSearchPanel;
                caseClass.Header = HeaderEditPanel;
                caseClass.PrisonDate = DateTime.MinValue;
                try
                {
                    await App.DataBase.SaveCasesAsync(caseClass);
                }
                catch
                {

                }
                finally
                {
                    Clear();
                }
            }
            else
            {

            }
        }

        private async void Clear()
        {
            selectedItem = null;
            CaseSearchPanel = null;
            HeaderEditPanel = null;
            LittigansSerachPanel = null;
            Items.Clear();
        }
    }
}
