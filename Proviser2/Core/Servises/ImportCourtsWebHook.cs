using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public static class ImportCourtsWebHook
    {
        public static async Task<bool> Import()
        {
            Debug.WriteLine("Start import courts");
            int k = 0;
            List<string> _courts = new List<string>(new string[] { "Заводський районний суд м.Дніпродзержинська", "Дніпровський районний суд м.Дніпродзержинська", "Баглійський районний суд м.Дніпродзержинська", "Дніпровський апеляційний суд", "Касаційний кримінальний суд Верховного Суду" });

            try
            {
                await foreach (var line in GetDataLines("https://dsa.court.gov.ua/open_data_files/91509/513/8faabdb91244be394947eb26f2153a1f.csv"))
                {
                    foreach (string _court in _courts)
                    {
                        if (line.Contains(_court))
                        {
                            if (line.Contains("207/") | line.Contains("208/") | line.Contains("209/"))
                            {
                                try
                                {
                                    CourtClass x = ClassConverter.TransformTextToCourtClass(line);
                                    x.Origin = "net";
                                    x.SaveDate = DateTime.Now;
                                    try
                                    {
                                        await App.DataBase.SaveCourtAsync(x);
                                        k++;
                                        Debug.WriteLine("save: " + line);

                                    }
                                    catch
                                    {
                                        try
                                        {
                                            var z = await App.DataBase.GetCourtByCaseAndDate(x.Case, x.Date);
                                            if (z != null)
                                            {
                                                if (z.Origin == "local")
                                                {
                                                    z.Origin = "full";
                                                    await App.DataBase.UpdateCourtsAsync(z);
                                                }                                          
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                catch
                                {
                                    Debug.WriteLine("transform error");
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception xx)
            {
                Debug.WriteLine(xx.Message.ToString());
                return false;
            }

        }

        static async Task<Stream> GetDataStream(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            return await response.Content.ReadAsStreamAsync();
        }

        static async IAsyncEnumerable<string> GetDataLines(string url)
        {
            using var _stream = await GetDataStream(url);
            using (var _reader = new StreamReader(_stream))
            {
                while (!_reader.EndOfStream)
                {
                    var line = _reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    yield return line;
                }
            }
        }
    }
}
