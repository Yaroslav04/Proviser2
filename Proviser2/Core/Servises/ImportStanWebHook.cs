using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public class ImportStanWebHook
    {

        public static async Task Import()
        {
            foreach (var url in GetSeparatedUrlListFromLink(@"https://dsa.court.gov.ua/dsa/inshe/oddata/532/"))
            {
                Debug.WriteLine($"NEW URL {url}");

                List<string> _courts = new List<string>(new string[] { "Заводський районний суд м.Дніпродзержинська", "Дніпровський районний суд м.Дніпродзержинська", "Баглійський районний суд м.Дніпродзержинська", "Дніпровський апеляційний суд", "Касаційний кримінальний суд", "Велика Палата Верховного Суду" });

                try
                {
                    await foreach (var line in GetDataLines(url))
                    {
                        foreach (string _court in _courts)
                        {
                            if (line.Contains(_court))
                            {
                                if (line.Contains("207/") | line.Contains("208/") | line.Contains("209/"))
                                {
                                    try
                                    {
                                        StanClass x = ClassConverter.TransformTextToStanClass(line);
                                        x.SaveDate = DateTime.Now;
                                        try
                                        {
                                            await App.DataBase.SaveStanAsync(x);
                                            Debug.WriteLine("save: " + line);

                                        }
                                        catch(Exception ex)
                                        {
                                            Debug.WriteLine("error: " + ex.Message.ToString());
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

                   
                }
                catch (Exception xx)
                {
                    Debug.WriteLine(xx.Message.ToString());       
                }
                finally
                {
                    FileManager.WriteLog("system", "stan", url);
                }
            }
        }

        public static List<string> GetSeparatedUrlListFromLink(string _url)
        {
            List<string> list = GetUrlListFromLink(_url);
            List<string> result = new List<string>();
            var savedStan = FileManager.GetSavedStan();
            if (savedStan.Count > 0)
            {
                foreach(var item in list)
                {
                    if (!savedStan.Contains(item))
                    {
                        result.Add(item);   
                    }   
                }
            }
            else
            {
                result = list;
            }
            return result;
        }

        public static List<string> GetUrlListFromLink(string _url)
        {
            List<string> list = new List<string>();
            try
            {
                WebRequest request = WebRequest.Create(_url);
                var r = request.GetResponse();
                if (r != null)
                {
                    Debug.WriteLine(r);
                    WebResponse response = r;
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line = "";
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains("open_data_files"))
                                {
                                    string s = GetURLFromLine(line);
                                    if (s != "")
                                    {
                                        list.Add(s);
                                    }
                                }
                            }
                        }
                    }
                    response.Close();
                    return list;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GetURLFromLine(string _value)
        {
            Regex regex = new Regex(@"(/532/)\w+");
            MatchCollection matches = regex.Matches(_value);
            if (matches.Count > 0)
            {
                return "https://dsa.court.gov.ua/open_data_files/91509/532/" + matches[0].Value.Replace("/532/", "") + ".csv";
            }
            return "";
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
