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
        public async Task<int> SaveAsync(T obj) => await CUD<T>.SaveAsync(obj, connection);
        public async Task<int> UpdateAsync(T obj) => await CUD<T>.UpdateAsync(obj, connection);
        public async Task<int> DeleteAsync(T obj) => await CUD<T>.DeleteAsync(obj, connection);
    }
}
