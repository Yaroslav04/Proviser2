using Proviser2.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public class LogDataBase : CUDDataBase<LogClass>
    {
        public LogDataBase(string _connectionString)
        {
            connection = new SQLiteAsyncConnection(_connectionString);
            connection.CreateTableAsync<LogClass>().Wait();
        }

        public async Task<List<LogClass>> GetListAsync()
        {
            return await connection.Table<LogClass>().ToListAsync();
        }

        public async Task<List<string>> GetSavedStanAsync()
        {
            List<string> result = new List<string>();
            var list = await connection.Table<LogClass>()
                .Where(x => x.Type == "download" & x.Teg == "stan" & x.Result == true).ToListAsync();

            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                foreach(var item in list) 
                {
                    result.Add(item.Value);
                }
                return result;
            }
        }

        public async Task<bool> IsDownloadNeed(string _teg)
        {
            var items = await connection.Table<LogClass>().Where(x => x.Type == "download" & x.Teg == _teg )
                .OrderByDescending(x => x.Date).ToListAsync();

            if (items.Count == 0)
            {           
                return true;
            }
            else
            {
                if ((DateTime.Now - items.FirstOrDefault().Date).TotalDays > 2)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> IsDownloadDecisionNeed(string _case)
        {
            var items = await connection.Table<LogClass>().Where(x => x.Type == "download" & x.Teg == "decision"
            & x.Value == _case)
                .OrderByDescending(x => x.Date).ToListAsync();

            if (items.Count == 0)
            {
                return true;
            }
            else
            {
                if ((DateTime.Now - items.FirstOrDefault().Date).TotalDays > 2)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<List<LogClass>> GetListSnifferExeption()
        {
            return await connection.Table<LogClass>().Where(x => x.Type == "sniffer_exeption").ToListAsync();
        }

    }
}
