using Proviser2.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
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
        readonly SQLiteAsyncConnection decisionDataBase;
        readonly SQLiteAsyncConnection journalDataBase;

        public DataBase(string _connectionString, List<string> _dataBaseName)
        {
            decisionDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[2]));
            decisionDataBase.CreateTableAsync<DecisionClass>().Wait();

            journalDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[3]));
            journalDataBase.CreateTableAsync<JournalClass>().Wait();

            courtsDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[0]));
            courtsDataBase.CreateTableAsync<CourtClass>().Wait();

            casesDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[1]));
            casesDataBase.CreateTableAsync<CaseClass>().Wait();
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
            catch
            {
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

        public Task<CourtClass> GetCourtsAsync(int _id)
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

        public Task<int> UpdateCasesAsync(CaseClass _case)
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

        public Task<CaseClass> GetCasesAsync(int _id)
        {
            return casesDataBase.Table<CaseClass>().Where(x => x.N == _id).FirstOrDefaultAsync();
        }

        public Task<CaseClass> GetCasesByCaseAsync(string _case)
        {
            return casesDataBase.Table<CaseClass>().Where(x => x.Case == _case).FirstOrDefaultAsync();
        }

        #endregion

        #region Functions

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

        #endregion
    }
}
