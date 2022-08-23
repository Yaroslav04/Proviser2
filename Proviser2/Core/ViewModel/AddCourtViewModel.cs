using Proviser2.Core.Model;
using Proviser2.Core.Servises;
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
    public class AddCourtViewModel : BaseViewModel
    {

        public AddCourtViewModel()
        {
            Title = "Додати судове засідання";
            Items = new ObservableCollection<CourtSoketClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            SaveCommand = new Command(Save);
        }

        private async void Save()
        {
            try
            {
                var courtClass = await App.DataBase.GetLastLocalCourtAsync(CaseId);
                if (courtClass != null)
                {
                    courtClass.Date = Convert.ToDateTime($"{Date.ToShortDateString()} {Time}");
                    courtClass.Origin = "local";
                    courtClass.SaveDate = DateTime.Now;
                    await App.DataBase.SaveCourtAsync(courtClass);
                    await Shell.Current.DisplayAlert("Реєстрція засідання", $"Зареєстровано {courtClass.Date}", "OK");
                }
                else
                {
                    List<string> courts = new List<string>(new string[] { "Заводський районний суд м.Дніпродзержинська", "Дніпровський районний суд м.Дніпродзержинська", "Баглійський районний суд м.Дніпродзержинська", "Дніпровський апеляційний суд", "Касаційний кримінальний суд Верховного Суду" });
                    string court = await Shell.Current.DisplayActionSheet("Додати судове засідання, виберіть суд", "Відміна", null, courts.ToArray());
                    string judge = await Shell.Current.DisplayPromptAsync("Додати судове засідання", $"Введіть суддю:", maxLength: 40);
                    string littigans = await Shell.Current.DisplayPromptAsync("Додати судове засідання", $"Введіть учасників:", maxLength: 100);
                    string category = await Shell.Current.DisplayPromptAsync("Додати судове засідання", $"Введіть категорію:", maxLength: 40);
                    courtClass = new CourtClass();
                    courtClass.Date = Convert.ToDateTime($"{Date.ToShortDateString()} {Time}");
                    courtClass.Origin = "local";
                    courtClass.SaveDate = DateTime.Now;
                    courtClass.Origin = "local";
                    courtClass.Case = CaseId;
                    courtClass.Court = court;
                    courtClass.Littigans = littigans;
                    courtClass.Category = category;
                    courtClass.SaveDate = DateTime.Now;
                    await App.DataBase.SaveCourtAsync(courtClass);
                    await Shell.Current.DisplayAlert("Реєстрція засідання", $"Зареєстровано {courtClass.Date}", "OK");

                }
            }
            catch
            {
                await Shell.Current.DisplayAlert("Реєстрція засідання", $"Помилка", "OK");
            }
            finally
            {
                Date = DateTime.Now;
                Time = TimeSpan.FromHours(12);
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        #region  Properties

        public ObservableCollection<CourtSoketClass> Items { get; }

        private string caseId;
        public string CaseId
        {
            get => caseId;
            set
            {
                SetProperty(ref caseId, value);
                SetHeader(value);
            }
        }

        private async void SetHeader(string _case)
        {
            Header = await App.DataBase.GetHeaderAsync(_case);
        }

        private string header;
        public string Header
        {
            get => header;
            set
            {
                SetProperty(ref header, value);
            }
        }

        private DateTime date = DateTime.Now;
        public DateTime Date
        {
            get => date;
            set
            {
                SetProperty(ref date, value);
            }
        }

        private TimeSpan time = TimeSpan.FromHours(12);
        public TimeSpan Time
        {
            get => time;
            set
            {
                SetProperty(ref time, value);
            }
        }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command SaveCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.DataBase.GetCourtsHearingOrderingByDateAsync();
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
