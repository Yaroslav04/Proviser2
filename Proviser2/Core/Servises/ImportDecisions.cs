using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public static class ImportDecisions
    {

        public static async Task Import()
        {
            var cases = await App.DataBase.GetCasesAsync();
            if (cases.Count > 0)
            {
                foreach (var c in cases)
                {
                    ParseERSR parseERSR = new ParseERSR();
                    parseERSR.AddCaseHeader(c.Case);
                    var result = await parseERSR.GetERSRCaseList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            try
                            {
                                DecisionClass decisionClass = new DecisionClass();
                                decisionClass.Case = item.Case;
                                decisionClass.DecisionDate = item.DecisionDate;                             
                                decisionClass.CriminalNumber = item.CriminalNumber;
                                decisionClass.Court = item.Court;
                                decisionClass.Id = item.Id;
                                decisionClass.Category = item.Category;
                                decisionClass.SaveDate = DateTime.Now;
                                decisionClass.LegalDate = item.LegalDate;
                                decisionClass.Content = item.Content;
                                decisionClass.DecisionType = item.DecisionType;
                                decisionClass.Judge = item.Judge;
                                decisionClass.URL = item.URL;
                                decisionClass.JudiciaryType = item.JudiciaryType;                 
                                await App.DataBase.SaveDecisionAsync(decisionClass);
                                Debug.WriteLine( $"save decison {item.URL}");
                            }
                            catch
                            {
                                try
                                {
                                    var existDecision = await App.DataBase.GetDecisionByIdAsync(item.Id);
                                    if (existDecision != null)
                                    {
                                        if (item.LegalDate != existDecision.LegalDate)
                                        {
                                            existDecision.LegalDate = item.LegalDate;
                                            existDecision.SaveDate = DateTime.Now;
                                            await App.DataBase.UpdateDecisionAsync(existDecision);
                                            Debug.WriteLine($"exist decision update {item.Case} {item.LegalDate}");
                                        }
                                    }
                                }
                                catch
                                {

                                }

                            }                                          
                        }
                    }
                }
            }   
        }
            public static async Task<string> GetResponseHTML(string _url)
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(_url);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }

            public static async Task<string> PostResponseHTML(string _url, Dictionary<string, string> _headers)
            {
                HttpClient client = new HttpClient();

                var content = new FormUrlEncodedContent(_headers);

                var response = await client.PostAsync(_url, content);

                var responseString = await response.Content.ReadAsStringAsync();

                return responseString;
            }
        
    }

    public static class ParseERSRCase
    {
        public static async Task<List<ERSRCaseClass>> GetERSRCaseList(List<ERSRClass> _list)
        {
            List<ERSRCaseClass> result = new List<ERSRCaseClass>();
            foreach (var item in _list)
            {
                result.Add(await GetERSRCase(item));
            }
            return result;
        }
        private static async Task<ERSRCaseClass> GetERSRCase(ERSRClass _ersr)
        {
            ERSRCaseClass result = new ERSRCaseClass(_ersr);
            string text = await ImportDecisions.GetResponseHTML(_ersr.URL);
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("Номер кримінального провадження в ЄРДР"))
                {
                    Regex regex = new Regex(@"\d\d\d\d\d\d\d\d\d\d\d\d\d\d\d\d\d");
                    MatchCollection matches = regex.Matches(line);
                    if (matches.Count > 0)
                    {
                        result.CriminalNumber = matches[0].Value;
                    }
                    else
                    {
                        regex = new Regex(@"\d(20)\d\d\d\d\d\d\d\d\d\d\d\d\d\d");
                        matches = regex.Matches(text);
                        if (matches.Count > 0)
                        {
                            result.CriminalNumber = matches[0].Value;
                        }
                    }
                }

                if (line.Contains("Дата набрання законної сили"))
                {
                    Regex regex = new Regex(@"\d\d(.)\d\d(.)\d\d\d\d");
                    MatchCollection matches = regex.Matches(line);
                    if (matches.Count > 0)
                    {
                        result.LegalDate = Convert.ToDateTime(matches[0].Value);
                    }
                    else
                    {
                        result.LegalDate = DateTime.MinValue;
                    }
                }

                if (line.Contains("Категорія&nbsp;справи"))
                {
                    string subresult = "";
                    Regex regex = new Regex(@"(</form>:)[\w, \W]*");
                    MatchCollection matches = regex.Matches(line);
                    if (matches.Count > 0)
                    {
                        subresult = matches[0].Value.Replace(".</b></td></tr>", "").Replace("</form>:", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    }

                    if (subresult != "")
                    {
                        result.Category = subresult;
                    }
                }
            }

            bool sw = false;
            List<string> grab = new List<string>();
            foreach (var line in lines)
            {
                if (sw)
                {
                    grab.Add(line);
                }

                if (line.Contains("<body>"))
                {
                    sw = true;
                }

                if (line.Contains("</body>"))
                {
                    sw = false;
                }

            }

            string content = "";
            foreach (var line in grab)
            {
                content = content + line + "\n";
            }

            var sx = content.Replace("&nbsp;", " ").Replace("&#171;", "").Replace("&#187;", "");

            string res = "";

            sw = false;

            foreach (char x in sx)
            {
                if (x == '>')
                {
                    sw = true;
                }

                if (x == '<')
                {
                    sw = false;
                }

                if (x != '>')
                {
                    if (sw)
                    {
                        res = res + x;
                    }
                }
            }


            result.Content = res.Replace("\t", "");

            return result;
        }
    }
    public class ParseERSR
    {
        private int pages;
        private int sort; //1 по убыванию // 0 ревалантность
        private List<HeaderClass> inputHeaders;
        private List<string> inputCourts;
        private string url = "https://reyestr.court.gov.ua/";
        public ParseERSR()
        {
            inputCourts = new List<string>();
            inputHeaders = new List<HeaderClass>();
            pages = 1000;
            sort = 1;
        }
        public ParseERSR(int _page, int _sort)
        {
            inputCourts = new List<string>();
            inputHeaders = new List<HeaderClass>();
            pages = _page;
            sort = _sort;
        }

        public async Task<List<ERSRClass>> GetERSRPageList()
        {
            List<ERSRClass> result = new List<ERSRClass>();
            foreach (var header in GetHeaders())
            {
                var subresult = GetERSRFromHTML(await ImportDecisions.PostResponseHTML(url, header));
                if (subresult.Count > 0)
                {
                    result.AddRange(subresult);
                }
            }

            result = result.Distinct().ToList();
            return result;
        }

        public async Task<List<ERSRCaseClass>> GetERSRCaseList()
        {
            return await ParseERSRCase.GetERSRCaseList(await GetERSRPageList());
        }

        public void AddHeader(string _header, string _value)
        {
            inputHeaders.Add(new HeaderClass
            {
                Header = _header,
                Content = _value
            });
        }

        public void AddSearchHeader(string _value)
        {
            inputHeaders.Add(new HeaderClass
            {
                Header = "SearchExpression",
                Content = _value
            });
        }

        public void AddCaseHeader(string _value)
        {
            inputHeaders.Add(new HeaderClass
            {
                Header = "CaseNumber",
                Content = _value
            });
        }

        public void AddCourt(string _court)
        {
            inputCourts.Add(_court);
        }

        public void SetLocalCourts()
        {
            inputCourts = new List<string> { "305", "309", "315" };
        }

        public void SetLocalFullCourts()
        {
            inputCourts = new List<string> { "305", "309", "315", "1054" };
        }

        private List<Dictionary<string, string>> GetHeaders()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            if (inputCourts.Count > 0)
            {

                foreach (var court in inputCourts)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("PagingInfo.ItemsPerPage", pages.ToString());
                    headers.Add("Sort", sort.ToString());
                    headers.Add("CSType", "2");
                    headers.Add("CourtName", court);
                    if (inputHeaders.Count > 0)
                    {
                        foreach (var input in inputHeaders)
                        {
                            headers.Add(input.Header, input.Content);
                        }
                    }
                    result.Add(headers);
                }
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("PagingInfo.ItemsPerPage", pages.ToString());
                headers.Add("Sort", sort.ToString());
                headers.Add("CSType", "2");
                if (inputHeaders.Count > 0)
                {
                    foreach (var input in inputHeaders)
                    {
                        headers.Add(input.Header, input.Content);
                    }
                }
                result.Add(headers);
            }

            return result;
        }

        private List<ERSRClass> GetERSRFromHTML(string _html)
        {
            var lines = _html.Split('\n');

            List<string> container = new List<string>();
            List<ERSRClass> list = new List<ERSRClass>();

            bool containerSensor = false;

            foreach (string line in lines)
            {
                if (line.Contains("<tr>"))
                {
                    containerSensor = true;
                }

                if (line.Contains("</tr>"))
                {
                    container.Add(line);
                    if (container.Count > 0)
                    {
                        bool sw = false;
                        foreach (var _c in container)
                        {
                            if (_c.Contains("/Review/"))
                            {
                                sw = true;
                            }
                        }

                        if (sw)
                        {
                            ERSRClass decision = new ERSRClass();

                            Regex regex = new Regex(@"(/Review/)\d*");
                            MatchCollection matches = regex.Matches(container[2]);
                            if (matches.Count > 0)
                            {
                                decision.Id = matches[0].Value.Replace("/Review/", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                                decision.URL = $"https://reyestr.court.gov.ua/Review/{matches[0].Value.Replace("/Review/", "").Replace("\t", "").Replace("\n", "").Replace("\r", "")}";
                            }

                            regex = new Regex(@"(>)\w+");
                            matches = regex.Matches(container[4]);
                            if (matches.Count > 0)
                            {
                                decision.DecisionType = matches[0].Value.Replace(">", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                            }

                            regex = new Regex(@"\d\d(.)\d\d(.)\d\d\d\d");
                            matches = regex.Matches(container[6]);
                            if (matches.Count > 0)
                            {
                                decision.DecisionDate = Convert.ToDateTime(matches[0].Value.Replace("\t", "").Replace("\n", "").Replace("\r", ""));
                            }

                            regex = new Regex(@"\d\d(.)\d\d(.)\d\d\d\d");
                            matches = regex.Matches(container[8]);
                            if (matches.Count > 0)
                            {
                                decision.PublicDate = Convert.ToDateTime(matches[0].Value.Replace("\t", "").Replace("\n", "").Replace("\r", ""));
                            }

                            regex = new Regex(@"(>)\w+");
                            matches = regex.Matches(container[10]);
                            if (matches.Count > 0)
                            {
                                decision.JudiciaryType = matches[0].Value.Replace(">", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                            }

                            //regex = new Regex(@"(>)\w+(/)\w+(/)\w+");
                            regex = new Regex(@"(>)(\S*)");
                            matches = regex.Matches(container[12]);
                            if (matches.Count > 0)
                            {
                                decision.Case = matches[0].Value.Replace(">", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                            }

                            regex = new Regex(@"(>)(\D*)");
                            matches = regex.Matches(container[14]);
                            if (matches.Count > 0)
                            {
                                decision.Court = matches[0].Value.Replace(">", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                            }

                            regex = new Regex(@"(>)(\D*)");
                            matches = regex.Matches(container[18]);
                            if (matches.Count > 0)
                            {
                                decision.Judge = matches[0].Value.Replace(">", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                            }


                            if (decision.Id != "")
                            {
                                list.Add(decision);
                            }
                        }
                    }
                    container.Clear();
                    containerSensor = false;
                }

                if (containerSensor == true)
                {
                    container.Add(line);
                }
            }

            return list;
        }
    }
    public class ERSRClass
    {
        public string Id { get; set; }
        public string DecisionType { get; set; }
        public DateTime DecisionDate { get; set; }
        public DateTime PublicDate { get; set; }
        public string JudiciaryType { get; set; }
        public string Case { get; set; }
        public string Court { get; set; }
        public string Judge { get; set; }
        public string URL { get; set; }

        public ERSRClass()
        {
            Id = "";
            DecisionType = "";
            DecisionDate = DateTime.MinValue;
            PublicDate = DateTime.MinValue;
            JudiciaryType = "";
            Case = "";
            Court = "";
            Judge = "";
            URL = "";
        }

        public bool Equals(ERSRClass ersr)
        {
            //Check whether the compared object is null.  
            if (Object.ReferenceEquals(ersr, null)) return false;

            //Check whether the compared object references the same data.  
            if (Object.ReferenceEquals(this, ersr)) return true;

            //Check whether the UserDetails' properties are equal.  
            return Id.Equals(ersr.Id);
        }

        // If Equals() returns true for a pair of objects   
        // then GetHashCode() must return the same value for these objects.  

        public override int GetHashCode()
        {

            //Get hash code for the UserName field if it is not null.  
            int hashN = Id == null ? 0 : Id.GetHashCode();

            //Calculate the hash code for the GPOPolicy.  
            return hashN;
        }
    }
    public class ERSRCaseClass : ERSRClass
    {
        public string Content { get; set; }
        public string CriminalNumber { get; set; }
        public DateTime LegalDate { get; set; }

        public string Category { get; set; }

        public ERSRCaseClass(ERSRClass _ersr)
        {
            Id = _ersr.Id;
            DecisionType = _ersr.DecisionType;
            DecisionDate = _ersr.DecisionDate;
            PublicDate = _ersr.PublicDate;
            JudiciaryType = _ersr.JudiciaryType;
            Case = _ersr.Case;
            Court = _ersr.Court;
            Judge = _ersr.Judge;
            URL = _ersr.URL;

            Content = "";
            CriminalNumber = "";
            LegalDate = DateTime.MinValue;
            Category = "";
        }
    }
    public class HeaderClass
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public HeaderClass()
        {
            Header = "";
            Content = "";
        }

        public HeaderClass(string _header, string _content)
        {
            Header = _header;
            Content = _content;
        }
    }

}
