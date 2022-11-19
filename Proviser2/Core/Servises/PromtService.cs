using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    public static class PromtService
    {
        public static async Task SimpleMessage(string title, string message)
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        }
        public static async Task AddCaseStart()
        {
            string description = "Пошук судової справи, для пошуку по прізвищу учасника (прокурора або обвинуваченого)" +
                "введіть прізвище в поле -🔎Пошук: учасник судового засідання-, регістр букв має значення," +
                "якщо не знайдено, очистіть поле для вводу та введіть номер судової справи (наприклад 208/1227/20)" +
                " в поле -🔎Пошук: номер судової справи-";
            await Shell.Current.DisplayAlert("Пошук судових справ", description, "OK");
        }
        public static async Task ShowPrisonSniffer(NotificationClass notificationClass)
        {
            string title = NotificationServise.GetTitleFromNotificationType(notificationClass.Type);
            await Shell.Current.DisplayAlert(title, notificationClass.Description, "OK");
            await NotificationServise.NotificationShowUpdate(notificationClass);
        }
        public static async Task ShowHearingSniffer(NotificationClass notificationClass)
        {
            string title = NotificationServise.GetTitleFromNotificationType(notificationClass.Type);
            await Shell.Current.DisplayAlert(title, notificationClass.Description, "OK");
            await NotificationServise.NotificationShowUpdate(notificationClass);
        }
        public static async Task<bool> IsNameSnifferSave(NotificationClass notificationClass)
        {
            string title = NotificationServise.GetTitleFromNotificationType(notificationClass.Type);
            bool answer = await Shell.Current.DisplayAlert(title, notificationClass.Description, "Так", "Ні");
            if (answer)
            {
                await NotificationServise.NotificationShowUpdate(notificationClass);
                return true;
            }
            {
                await NotificationServise.NotificationShowUpdate(notificationClass);
                return false;
            }    
        }
        public static async Task AddNameSniffer()
        {
            if (await SnifferServise.IsSetNameSniffer())
            {
                string title = "Додати дані користувача для пошуку";
                string name = await Shell.Current.DisplayPromptAsync(title, "Введіть своє прізвище та ім'я", maxLength: 30);
                if (!String.IsNullOrWhiteSpace(name))
                {
                    await SnifferServise.AddNameSniffer(name);
                    await Shell.Current.DisplayAlert(title, "Данні для пошуку встановлені", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert(title, $"Помилка, поле порожнє", "OK");
                }
            }
        }
    }
}
