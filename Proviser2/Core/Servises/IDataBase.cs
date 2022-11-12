using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public interface IDataBase <T>
    {
       Task<int> SaveAsync(T obj);
        Task<int> UpdateAsync(T obj);
        Task<int> DeleteAsync(T obj);
        Task<List<T>> GetListAsync();
    }
}
