using Proviser2.Core.Model;
using Proviser2.Core.Servises.UserInterfaseConverters;
using Proviser2.Core.View;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class CaseViewModel : BaseViewModel
    {
        public CaseViewModel()
        {
            FastAddCommand = new Command(FastAdd);
            OpenEventsCommand = new Command(OpenEvents);
        }

        #region  Properties

        private string caseId;
        public string CaseId
        {
            get => caseId;
            set
            {
                SetProperty(ref caseId, value);
                LoadCase(value);
            }
        }

        private string headerMainPanel;
        public string HeaderMainPanel
        {
            get => headerMainPanel;
            set
            {
                SetProperty(ref headerMainPanel, value);          
            }
        }

        private string caseMainPanel;
        public string CaseMainPanel
        {
            get => caseMainPanel;
            set
            {
                SetProperty(ref caseMainPanel, value);
            }
        }

        private string noteMainPanel;
        public string NoteMainPanel
        {
            get => noteMainPanel;
            set
            {
                SetProperty(ref noteMainPanel, value);
            }
        }

        private string prisonDateMainPanel;
        public string PrisonDateMainPanel
        {
            get => prisonDateMainPanel;
            set
            {
                SetProperty(ref prisonDateMainPanel, value);
            }
        }

        private string criminalNumberMainPanel;
        public string CriminalNumberMainPanel
        {
            get => criminalNumberMainPanel;
            set
            {
                SetProperty(ref criminalNumberMainPanel, value);
            }
        }

        private string mainCaseMainPanel;
        public string MainCaseMainPanel
        {
            get => mainCaseMainPanel;
            set
            {
                SetProperty(ref mainCaseMainPanel, value);
            }
        }

        private string courtMainPanel;
        public string CourtMainPanel
        {
            get => courtMainPanel;
            set
            {
                SetProperty(ref courtMainPanel, value);
            }
        }

        private string judgeMainPanel;
        public string JudgeMainPanel
        {
            get => judgeMainPanel;
            set
            {
                SetProperty(ref judgeMainPanel, value);
            }
        }

        private string littigansMainPanel;
        public string LittigansMainPanel
        {
            get => littigansMainPanel;
            set
            {
                SetProperty(ref littigansMainPanel, value);
            }
        }

        private string categoryMainPanel;
        public string CategoryMainPanel
        {
            get => categoryMainPanel;
            set
            {
                SetProperty(ref categoryMainPanel, value);
            }
        }

        #endregion


        #region Commands

        public Command FastAddCommand { get; }
        public Command OpenEventsCommand { get; }
        public Command OpenCourtsCommand { get; }
        public Command OpenDecisionCommand { get; }

        #endregion


        private void FastAdd()
        {
            
        }

        private async void OpenEvents()
        {
            await Shell.Current.GoToAsync($"{nameof(EventsListPage)}?{nameof(CaseViewModel.CaseId)}={CaseId}");

        }

        private async void LoadCase(string _case)
        {
            var item = await App.DataBase.GetCasesByCaseAsync(_case);
            Title = item.Header;
            HeaderMainPanel = item.Header;
            CaseMainPanel = item.Case;
            NoteMainPanel = item.Note;
            PrisonDateMainPanel = PrisonDateConverter.GetBeautifyPrisonDate(item.PrisonDate);
            CriminalNumberMainPanel = item.CriminalNumber;
            MainCaseMainPanel = item.MainCase;
            var court = await App.DataBase.GetLastLocalCourtAsync(_case);
            CourtMainPanel = court.Court;
            JudgeMainPanel = court.Judge;
            LittigansMainPanel = court.Littigans;
            CategoryMainPanel = court.Category;
        }
    }
}
