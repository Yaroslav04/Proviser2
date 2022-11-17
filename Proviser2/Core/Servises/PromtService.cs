using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    public static class PromtService
    {
        public static async Task AddCaseStart()
        {
            string description = "Пошук судової справи, для пошуку по прізвищу учасника (прокурора або обвинуваченого)" +
                "введіть прізвище в поле -🔎Пошук: учасник судового засідання-, регістр букв має значення," +
                "якщо не знайдено, очистіть поле для вводу та введіть номер судової справи (наприклад 208/1227/20)" +
                " в поле -🔎Пошук: номер судової справи-";
            await Shell.Current.DisplayAlert("Пошук судових справ", description, "OK");
        }
    }
}
