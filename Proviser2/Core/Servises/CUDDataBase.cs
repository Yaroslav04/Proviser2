using Proviser2.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public class CUDDataBase<T>
    {
        public SQLiteAsyncConnection connection;
        public CUDDataBase(string _connectionString)
        {
            connection = new SQLiteAsyncConnection(_connectionString);
            connection.CreateTableAsync<LogClass>().Wait();
        }

        public async Task<int> SaveAsync(LogClass obj) => await CUD<LogClass>.SaveAsync(obj, connection);
        public async Task<int> UpdateAsync(LogClass obj) => await CUD<LogClass>.UpdateAsync(obj, connection);
        public async Task<int> DeleteAsync(LogClass obj) => await CUD<LogClass>.DeleteAsync(obj, connection);
    }
}
