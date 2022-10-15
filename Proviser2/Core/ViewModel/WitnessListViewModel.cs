using Proviser2.Core.Model;
using Proviser2.Core.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.ViewModel
{
    [QueryProperty(nameof(CaseId), nameof(CaseId))]
    public class WitnessListViewModel : BaseViewModel
    {
        public WitnessListViewModel()
        {
            Title = "Свідки";
            Items = new ObservableCollection<WitnessClass>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<WitnessClass>(OnItemSelected);
            AddItemCommand = new Command(async () => await AddWitness());
        }

        public void OnAppearing()
        {
            IsBusy = true;
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

        public ObservableCollection<WitnessClass> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command<WitnessClass> ItemTapped { get; }

        public Command AddItemCommand { get; }

        #endregion

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();

                var witness = await App.DataBase.GetWitnessByCaseAsync(CaseId);
                if (witness.Count > 0)
                {
                    foreach(var w in witness)
                    {
                        Items.Add(w);
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
        async void OnItemSelected(WitnessClass item)
        {
            if (item == null)
                return;
            string[] menu = { "Змінити активність", "Видалити" };
            string answer = await Shell.Current.DisplayActionSheet("Оберіть дію", "Cancel", null, menu);

            if (answer == menu[0])
            {
                if (item.Status)
                {
                    item.Status = false;
                    await App.DataBase.UpdateWitnessAsync(item);
                }
                else
                {
                    item.Status = true;
                    await App.DataBase.UpdateWitnessAsync(item);
                }
            }

            if (answer == menu[1])
            {
                bool _answer = await Shell.Current.DisplayAlert("Видалення", $"Видалити {item.Name}", "Так", "Ні");
                if (_answer)
                {
                    await App.DataBase.DeleteWitnessAsync(item);
                }
            }
   
            IsBusy = true;
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
                IsBusy = true;
                return;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Додати свідка", $"Помилка {ex.Message}", "OK");
                return;
            }
        }
    }


}
