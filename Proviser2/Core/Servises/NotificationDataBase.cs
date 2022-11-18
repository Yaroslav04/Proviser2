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

        public async Task<List<NotificationClass>> GetNotificateListAsync()
        {
            return await connection.Table<NotificationClass>().Where(x => x.IsNotificate == true).ToListAsync();
        }

        public async Task<List<NotificationClass>> GetExecuteListAsync(string _type)
        {
            return await connection.Table<NotificationClass>().Where(x => x.Type == _type
            & x.IsExecute == true).ToListAsync();
        }

        public async Task<List<NotificationClass>> GetExecuteListAsync(string _type, int _day)
        {
            return await connection.Table<NotificationClass>().Where(x => x.Type == _type & x.Day == _day
            & x.IsExecute == true).ToListAsync();
        }
    }
}
