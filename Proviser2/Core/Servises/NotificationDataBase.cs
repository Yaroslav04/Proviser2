using Proviser2.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public class NotificationDataBase : CUDDataBase<NotificationClass>
    {
        public NotificationDataBase(string _connectionString)
        {
            connection = new SQLiteAsyncConnection(_connectionString);
            connection.CreateTableAsync<NotificationClass>().Wait();
        }
        public async Task<List<NotificationClass>> GetListAsync()
        {
            return await connection.Table<NotificationClass>().ToListAsync();
        }
    }
}
