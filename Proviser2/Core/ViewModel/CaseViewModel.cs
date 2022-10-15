using Proviser2.Core.Model;
using Proviser2.Core.Servises;
using Proviser2.Core.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Xml.Linq;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class CaseViewModel : BaseViewModel
    {
        public CaseViewModel()
        {
            FastAddCommand = new Command(FastAdd);
            OpenEventsCommand = new Command(OpenEvents);
            OpenCourtsCommand = new Command(OpenCourts);
            OpenDecisionCommand = new Command(OpenDecisions);
            OpenStanCommand = new Command(OpenStan);
            OpenWitnessCommand = new Command(OpenWitness);
            DeleteCommand = new Command(Delete);
            UpdateCommand = new Command(Update);
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
        public Command OpenStanCommand { get; }

        public Command OpenWitnessCommand { get; }
        public Command DeleteCommand { get; }
        public Command UpdateCommand { get; }

        #endregion

        private async void FastAdd()
        {
            List<string> functions = new List<string> {
                "Додати судове засідання", "Додати дату тримання під вартою", "Додати примітку",
                "Додати свідка", "Додати судову подію", "Завантажити судові рішення"
            };
            string result = await Shell.Current.DisplayActionSheet("Швидкий доступ", "Відміна", null, functions.ToArray());
            switch (result)
            {
                case "Додати судове засідання":
                    await OpenAddCourt();
                    break;

                case "Додати дату тримання під вартою":
                    await AddPrisonDate();
                    break;

                case "Додати примітку":
                    await UpdateNote();
                    break;

                case "Додати свідка":
                    await AddWitness();                 
                    break;

                case "Додати судову подію":
                    await AddEvent();
                    break;

                case "Завантажити судові рішення":
                    await Task.Run(() => ImportDecisions.Import(CaseId));
                    await Shell.Current.DisplayAlert("Завантаження", "Рішення завантажено", "OK");
                    break;          
            }
        }

        private async Task AddEvent()
        {
            await Shell.Current.GoToAsync($"{nameof(EventPage)}?{nameof(EventViewModel.EventId)}=new\t{CaseId}");
        }

        private async Task AddWitness()
        {
            string _type = await Shell.Current.DisplayActionSheet("Виберіть тип свідка", "Cancel", null, App.WitnessTypes.ToArray());
            if (String.IsNullOrWhiteSpace(_type) | _type == "Cancel")
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Не обраний тип", "OK");
                return;
            }

            string _name = await Shell.Current.DisplayPromptAsync($"Додати свідка", $"Введіть П.І.Б. свідка", maxLength: 30);
            if (String.IsNullOrWhiteSpace(_name))
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Не вказано П.І.Б.", "OK");
                return;
            }

            string _birthDate = await Shell.Current.DisplayPromptAsync($"Додати свідка", $"Введіть дату народження свідка", maxLength: 10);
            if (String.IsNullOrWhiteSpace(_birthDate))
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Не вказано дату народження свідка", "OK");
                return;
            }

            string _location = await Shell.Current.DisplayPromptAsync($"Додати свідка", $"Введіть місце проживання свідка");
            if (String.IsNullOrWhiteSpace(_location))
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Не вказано місце проживання свідка", "OK");
                return;
            }

            string _work = await Shell.Current.DisplayPromptAsync($"Додати свідка", $"Введіть місце роботи свідка");
            if (String.IsNullOrWhiteSpace(_work))
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Не вказано місце роботи свідка", "OK");
                return;
            }

            string _contact = await Shell.Current.DisplayPromptAsync($"Додати свідка", $"Введіть засоби зв'язку свідка");
            if (String.IsNullOrWhiteSpace(_contact))
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Не вказано контакти свідка", "OK");
                return;
            }

            try
            {
                await App.DataBase.SaveWitnessAsync(
                    new WitnessClass
                    {
                        Case = CaseId,
                        Type = _type,
                        Name = _name,
                        BirthDate = _birthDate,
                        Location = _location,
                        Work = _work,
                        Contact = _contact,
                        Status = true                       
                    });
                await Shell.Current.DisplayAlert("Додати свідка", "Свідка додано", "OK");
                return;
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Помилка {ex.Message}", "OK");
                return;
            }
        }

        private async Task UpdateNote()
        {
            string result = await Shell.Current.DisplayPromptAsync("Редагування примітки", $"Редагування примітки для {HeaderMainPanel}:", accept: "OK", cancel: "Відміна", maxLength: 100, initialValue: NoteMainPanel);
            if (result != "Відміна")
            {
                try
                {
                    var _cases = await App.DataBase.GetCasesByCaseAsync(CaseId);
                    _cases.Note = result;
                    await App.DataBase.UpdateCaseAsync(_cases);
                    await Shell.Current.DisplayAlert("Успішно", $"Примтіка оновлена", "OK");
                    NoteMainPanel = result;
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Помилка", $"Помилка оновлення примітки", "OK");
                }
            }
        }

        private async Task AddPrisonDate()
        {
            string result = await Shell.Current.DisplayPromptAsync("Додати дату тримання під вартою", $"Введіть дату тримання під вартою для {HeaderMainPanel}:", maxLength: 10);

            if (String.IsNullOrWhiteSpace(result))
            {
                return;
            }
            else
            {

                if (TextManager.Date(result))
                {
                    if (Convert.ToDateTime(result) >= DateTime.Now)
                    {
                        var item = await App.DataBase.GetCasesByCaseAsync(CaseId);
                        if (item != null)
                        {
                            item.PrisonDate = Convert.ToDateTime(result);
                            await App.DataBase.UpdateCaseAsync(item);
                            await Shell.Current.DisplayAlert("Успішно", "Дата тримання під вартою додана", "OK");
                            PrisonDateMainPanel = TextManager.GetBeautifyPrisonDate(Convert.ToDateTime(result));
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Помилка", "Дата в минулому", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Помилка", "Невірно зазначено формат дати", "OK");
                }
            }
        }

        private async Task OpenAddCourt()
        {
            await Shell.Current.GoToAsync($"{nameof(AddCourtPage)}?{nameof(AddCourtViewModel.CaseId)}={CaseId}");
        }

        private async void OpenEvents()
        {
            await Shell.Current.GoToAsync($"{nameof(EventsListPage)}?{nameof(EventsListViewModel.CaseId)}={CaseId}");
        }

        private async void OpenDecisions()
        {
            await Shell.Current.GoToAsync($"{nameof(DecisionsListPage)}?{nameof(DecisionsListViewModel.CaseId)}={CaseId}");
        }

        private async void OpenStan()
        {
            await Shell.Current.GoToAsync($"{nameof(StanListPage)}?{nameof(StanListViewModel.CaseId)}={CaseId}");
        }
        private async void OpenWitness()
        {
            await Shell.Current.GoToAsync($"{nameof(WitnessListPage)}?{nameof(WitnessListViewModel.CaseId)}={CaseId}");
        }

        private async void OpenCourts()
        {
            await Shell.Current.GoToAsync($"{nameof(CourtsFromCaseListPage)}?{nameof(CourtsFromCaseListViewModel.CaseId)}={CaseId}");
        }

        private async void Delete()
        {
            bool answer = await Shell.Current.DisplayAlert("Видалення", $"Видалити {HeaderMainPanel} {CaseMainPanel}", "Так", "Ні");
            if (answer) 
            {
                await App.DataBase.DeleteCasesAsync(await App.DataBase.GetCasesByCaseAsync(CaseId));
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void Update()
        {
            CaseClass caseClass = await App.DataBase.GetCasesByCaseAsync(CaseId);
            caseClass.MainCase = MainCaseMainPanel;
            caseClass.CriminalNumber = CriminalNumberMainPanel;
            caseClass.Header = HeaderMainPanel;
            await App.DataBase.UpdateCaseAsync(caseClass);
        }

        private async void LoadCase(string _case)
        {
            CaseClass item = await App.DataBase.GetCasesByCaseAsync(_case);

            Title = item.Header;
            HeaderMainPanel = item.Header;
            CaseMainPanel = item.Case;
            NoteMainPanel = item.Note;
            PrisonDateMainPanel = TextManager.GetBeautifyPrisonDate(item.PrisonDate);
            CriminalNumberMainPanel = item.CriminalNumber;
            MainCaseMainPanel = item.MainCase;
            var court = await App.DataBase.GetLastLocalCourtAsync(_case);
            if (court != null)
            {
                CourtMainPanel = court.Court;
                JudgeMainPanel = court.Judge;
                LittigansMainPanel = court.Littigans;
                CategoryMainPanel = court.Category;
            }
        }
    }
}
