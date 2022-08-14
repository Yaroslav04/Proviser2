using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(EventId), nameof(EventId))]
    public class EventViewModel : BaseViewModel
    {

        public EventViewModel()
        {
            SaveCommand = new Command(Save);
            DeleteCommand = new Command(Delete);
            Events = new ObservableCollection<string>
            {
                "допит свідка"
            };
        }

        #region Properties

        private string eventCase { get; set; }
        private string eventN { get; set; }

        private string eventId;
        public string EventId
        {
            get => eventId;
            set
            {
                SetProperty(ref eventId, value);
                eventCase = value.Split("\t")[1];
                eventN = value.Split("\t")[0];
                Load();
            }
        }

        private string dateMainPanel;
        public string DateMainPanel
        {
            get => dateMainPanel;
            set
            {
                SetProperty(ref dateMainPanel, value);
            }
        }
        public ObservableCollection<string> Events { get; }

        private string selectedEventMainPanel;
        public string SelectedEventMainPanel
        {
            get => selectedEventMainPanel;
            set
            {
                SetProperty(ref selectedEventMainPanel, value);
            }
        }

        private string descriprtionMainPanel;
        public string DescriprtionMainPanel
        {
            get => descriprtionMainPanel;
            set
            {
                SetProperty(ref descriprtionMainPanel, value);
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

        private string headerMainPanel;
        public string HeaderMainPanel
        {
            get => headerMainPanel;
            set
            {
                SetProperty(ref headerMainPanel, value);
            }
        }

        #endregion


        #region Commands
        public Command SaveCommand { get; }
        public Command DeleteCommand { get; }

        #endregion

        private async void Load()
        {
            if (eventN != "new")
            {
                Title = "Редагування події";
                var item = await App.DataBase.GetEventAsync(Convert.ToInt32(eventN));
                if (item != null)
                {
                    HeaderMainPanel = await App.DataBase.GetHeaderAsync(eventCase);
                    DateMainPanel = item.Date.ToShortDateString();
                    SelectedEventMainPanel = item.Event;
                    DescriprtionMainPanel = item.Description;
                    CaseMainPanel = item.Case;
                }
            }
            else
            {
                Title = "Реєстрація події";
                CaseMainPanel = eventCase;
                HeaderMainPanel = await App.DataBase.GetHeaderAsync(eventCase);
                DateMainPanel = DateTime.Now.ToShortDateString();
            }

        }

        private async void Delete()
        {
            if (eventN != "new")
            {
                EventClass eventClass = await App.DataBase.GetEventAsync(Convert.ToInt32(eventN));
                try
                {
                    await App.DataBase.DeleteEventAsync(eventClass);
                    await Shell.Current.GoToAsync("..");
                }
                catch
                {

                }
            }
        }

        private async void Save(object obj)
        {
            if (eventN == "new")
            {
                EventClass eventClass = new EventClass();
                eventClass.Case = eventCase;
                eventClass.Date = Convert.ToDateTime(DateMainPanel);
                eventClass.Event = SelectedEventMainPanel;
                eventClass.Description = DescriprtionMainPanel;

                try
                {
                    await App.DataBase.SaveEventAsync(eventClass);
                    await Shell.Current.GoToAsync("..");
                }
                catch
                {

                }
            }
            else
            {
                EventClass eventClass = await App.DataBase.GetEventAsync(Convert.ToInt32(eventN));
                eventClass.Date = Convert.ToDateTime(DateMainPanel);
                eventClass.Event = SelectedEventMainPanel;
                eventClass.Description = DescriprtionMainPanel;

                try
                {
                    await App.DataBase.UpdateEventAsync(eventClass);
                }
                catch
                {

                }
                finally
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
        }
    }
}
