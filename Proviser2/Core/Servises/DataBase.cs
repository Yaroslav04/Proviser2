﻿using Proviser2.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection courtsDataBase;
        readonly SQLiteAsyncConnection casesDataBase;
        readonly SQLiteAsyncConnection decisionsDataBase;
        readonly SQLiteAsyncConnection eventsDataBase;
        readonly SQLiteAsyncConnection configDataBase;
        readonly SQLiteAsyncConnection legasyDataBase;

        public DataBase(string _connectionString, List<string> _dataBaseName)
        {

            courtsDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[0]));
            courtsDataBase.CreateTableAsync<CourtClass>().Wait();

            casesDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[1]));
            casesDataBase.CreateTableAsync<CaseClass>().Wait();

            decisionsDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[2]));
            decisionsDataBase.CreateTableAsync<DecisionClass>().Wait();

            eventsDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[3]));
            eventsDataBase.CreateTableAsync<EventClass>().Wait();

            configDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[4]));
            configDataBase.CreateTableAsync<ConfigClass>().Wait();

            legasyDataBase = new SQLiteAsyncConnection(@"/storage/emulated/0/Proviser/CourtsDataBase.db3");
            legasyDataBase.CreateTableAsync<Courts>().Wait();
        }

        #region Court
        public Task<int> SaveCourtAsync(CourtClass _court)
        {
            try
            {
                return courtsDataBase.InsertAsync(_court);
            }
            catch
            {
                return null;
            }
        }
        public Task<int> DeleteCourtAsync(CourtClass _court)
        {
            try
            {
                return courtsDataBase.DeleteAsync(_court);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }
        public Task<int> UpdateCourtsAsync(CourtClass _court)
        {
            try
            {
                return courtsDataBase.UpdateAsync(_court);
            }
            catch
            {
                return null;
            }

        }
        public Task<List<CourtClass>> GetCourtsAsync()
        {
            return courtsDataBase.Table<CourtClass>().ToListAsync();
        }
        public async Task<List<CourtClass>> GetLastCourtsAsync()
        {
            var cases = await GetCasesAsync();
            if (cases.Count == 0)
            {
                return null;
            }
            else
            {
                var casesNum = new List<string>();
                foreach (var item in cases)
                {
                    casesNum.Add(item.Case);
                }
                return await courtsDataBase.Table<CourtClass>().Where(x => x.Origin != "local")
                    .Where(x => casesNum.Contains(x.Case))
                    .OrderByDescending(x => x.SaveDate).Take(25).ToListAsync();
            }    
        }
        public Task<CourtClass> GetCourtAsync(int _id)
        {
            return courtsDataBase.Table<CourtClass>().Where(x => x.N == _id).FirstOrDefaultAsync();
        }
        public Task<List<CourtClass>> GetCourtsAsync(string _case)
        {
            return courtsDataBase.Table<CourtClass>().Where(x => x.Case == _case).ToListAsync();
        }
        public Task<List<CourtClass>> GetCourtsByLittigansAsync(string _value)
        {
            return courtsDataBase.Table<CourtClass>().Where(x => x.Littigans.Contains(_value)).ToListAsync();
        }
        public async Task<CourtClass> GetLastLocalCourtAsync(string _case)
        {
            var courts = await GetCourtsAsync(_case);
            if (courts.Count > 0)
            {
                if (courts.Last().Court != "Дніпровський апеляційний суд")
                {
                    return courts.Last();
                }
                else
                {
                    courts = courts.OrderBy(x => x.Date).ToList();
                    var subresult = courts.Where(x => x.Court != "Дніпровський апеляційний суд").ToList();
                    if (subresult.Count > 0)
                    {
                        return subresult.Last();
                    }
                    else
                    {
                        return courts.Last();
                    }
                }
            }
            else 
            {
                return null;           
            }
        }
        public async Task<List<CourtClass>> GetCourtsHearingOrderingByDateAsync()
        {
            List<CourtClass> _courts = new List<CourtClass>();
            List<CourtClass> _result = new List<CourtClass>();

            var _cases = await GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var _case in _cases)
                {
                    var _list = await GetCourtsAsync(_case.Case);
                    if (_list.Count > 0)
                    {
                        _courts.AddRange(_list);
                    }
                }

                _courts = _courts.OrderBy(x => x.Date).ToList();

                if (_courts.Count > 0)
                {
                    foreach (var _item in _courts)
                    {
                        if (_item.Date >= DateTime.Today)
                        {
                            _result.Add(_item);
                        }
                    }

                    return _result;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }

        }
        public async Task<List<CourtClass>> GetNotExistCourtsByLittigans(string _name)
        {
            List<string> casesFromCourtListByLittigans = new List<string>();

            List<CourtClass> courtsList = await GetCourtsByLittigansAsync(_name);

            if (courtsList.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var item in courtsList)
                {
                    casesFromCourtListByLittigans.Add(item.Case);
                }

                casesFromCourtListByLittigans = casesFromCourtListByLittigans.Distinct().ToList();
            }

            List<string> casesResult = new List<string>();

            if (casesFromCourtListByLittigans.Count == 0)
            {
                return null;
            }
            else
            {
                var savedCases = await GetCasesAsync();

                if (savedCases.Count == 0)
                {
                    foreach (var item in casesFromCourtListByLittigans)
                    {
                        casesResult.Add(item);
                    }
                }
                else
                {
                    foreach (var item in casesFromCourtListByLittigans)
                    {

                        bool sw = false;

                        foreach (var subitem in savedCases)
                        {
                            if (item == subitem.Case)
                            {
                                sw = true;
                            }
                        }

                        if (sw == false)
                        {
                            casesResult.Add(item);
                        }
                    }
                }
            }

            if (casesResult.Count == 0)
            {
                return null;
            }
            else
            {
                List<CourtClass> courtsResult = new List<CourtClass>();

                foreach (var c in casesResult)
                {
                    CourtClass r = await GetLastLocalCourtAsync(c);
                    if (r != null)
                    {
                        courtsResult.Add(r);
                    }
                }

                if (courtsResult.Count == 0)
                {
                    return null;
                }
                else
                {
                    courtsResult = courtsResult.OrderByDescending(x => x.Date).ToList();
                    return courtsResult;
                }
            }
        }

        public async Task<DateTime> GetLastDownloadCourtDate()
        {
            var last = await courtsDataBase.Table<CourtClass>()
                .Where(x => x.Origin == "net")
                .OrderByDescending(x => x.SaveDate)
                .FirstOrDefaultAsync();
            if (last != null)
            {
                return last.SaveDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        #endregion

        #region Case

        public Task<int> SaveCasesAsync(CaseClass _case)
        {
            try
            {
                return casesDataBase.InsertAsync(_case);
            }
            catch
            {
                return null;
            }
        }
        public Task<int> DeleteCasesAsync(CaseClass _case)
        {
            try
            {
                return casesDataBase.DeleteAsync(_case);
            }
            catch
            {
                return null;
            }
        }
        public Task<int> UpdateCaseAsync(CaseClass _case)
        {
            try
            {
                return casesDataBase.UpdateAsync(_case);
            }
            catch
            {
                return null;
            }
        }
        public Task<List<CaseClass>> GetCasesAsync()
        {
            return casesDataBase.Table<CaseClass>().ToListAsync();
        }
        public Task<CaseClass> GetCaseAsync(int _id)
        {
            return casesDataBase.Table<CaseClass>().Where(x => x.N == _id).FirstOrDefaultAsync();
        }
        public Task<CaseClass> GetCasesByCaseAsync(string _case)
        {
            return casesDataBase.Table<CaseClass>().Where(x => x.Case == _case).FirstOrDefaultAsync();
        }
        public async Task<string> GetHeaderAsync(string _case)
        {
            var subcase = await GetCasesByCaseAsync(_case);
            if (subcase != null)
            {
                return subcase.Header;
            }
            else
            {
                return string.Empty;
            }
           
        }

        #endregion

        #region Events

        public Task<int> SaveEventAsync(EventClass _event)
        {
            try
            {
                return eventsDataBase.InsertAsync(_event);
            }
            catch
            {
                return null;
            }
        }

        public Task<int> DeleteEventAsync(EventClass _event)
        {
            try
            {
                return eventsDataBase.DeleteAsync(_event);
            }
            catch
            {
                return null;
            }

        }
        public Task<int> UpdateEventAsync(EventClass _event)
        {
            try
            {
                return eventsDataBase.UpdateAsync(_event);
            }
            catch
            {
                return null;
            }
        }

        public Task<List<EventClass>> GetEventsAsync(string _case)
        {
            return eventsDataBase.Table<EventClass>().Where(x => x.Case == _case).OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<EventClass> GetEventAsync(int _id)
        {
            return await eventsDataBase.Table<EventClass>().Where(x => x.N == _id).FirstOrDefaultAsync();
        }

        #endregion

        #region Decisions

        public Task<int> SaveDecisionAsync(DecisionClass _decision)
        {
            try
            {
                return decisionsDataBase.InsertAsync(_decision);
            }
            catch
            {
                return null;
            }
        }

        public Task<int> DeleteDecisionAsync(DecisionClass _decision)
        {
            try
            {
                return decisionsDataBase.DeleteAsync(_decision);
            }
            catch
            {
                return null;
            }

        }
        public Task<int> UpdateDecisionAsync(DecisionClass _decision)
        {
            try
            {
                return decisionsDataBase.UpdateAsync(_decision);
            }
            catch
            {
                return null;
            }
        }

        public Task<List<DecisionClass>> GetDecisionsAsync(string _case)
        {
            return decisionsDataBase.Table<DecisionClass>().Where(x => x.Case == _case).OrderByDescending(x => x.DecisionDate).ToListAsync();
        }

        public async Task<DecisionClass> GetDecisionAsync(int _id)
        {
            return await decisionsDataBase.Table<DecisionClass>().Where(x => x.N == _id).FirstOrDefaultAsync();
        }

        public async Task<DecisionClass> GetDecisionByIdAsync(string _id)
        {
            return await decisionsDataBase.Table<DecisionClass>().Where(x => x.Id == _id).FirstOrDefaultAsync();
        }

        public async Task<List<DecisionClass>> GetLastDecisionsAsync()
        {
            var cases = await GetCasesAsync();
            if (cases.Count == 0)
            {
                return null;
            }
            else
            {
                var casesNum = new List<string>();
                foreach (var item in cases)
                {
                    casesNum.Add(item.Case);
                }
                return await decisionsDataBase.Table<DecisionClass>()
                    .Where(x => casesNum.Contains(x.Case))
                    .OrderByDescending(x => x.SaveDate).Take(25).ToListAsync();
            }
        }

        public async Task<DateTime> GetLastDownloadDecisionDate()
        {
            var last = await decisionsDataBase.Table<DecisionClass>()
                .OrderByDescending(x => x.SaveDate)
                .FirstOrDefaultAsync();

            if (last != null)
            {
                return last.SaveDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        #endregion

        #region Config

        public Task<int> SaveConfigAsync(ConfigClass _config)
        {
            try
            {
                return configDataBase.InsertAsync(_config);
            }
            catch
            {
                return null;
            }
        }

        public Task<int> DeleteConfigAsync(ConfigClass _config)
        {
            try
            {
                return configDataBase.DeleteAsync(_config);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }
        public Task<int> UpdateConfigAsync(ConfigClass _config)
        {
            try
            {
                return configDataBase.UpdateAsync(_config);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<ConfigClass>> GetConfigAsync(string _type)
        {
            return configDataBase.Table<ConfigClass>().Where(x => x.Type == _type).ToListAsync();
        }

        #endregion

        #region Legasy

        public Task<List<Courts>> GetOldCourtsAsync()
        {
            return legasyDataBase.Table<Courts>().ToListAsync();
        }

        #endregion

    }
}
