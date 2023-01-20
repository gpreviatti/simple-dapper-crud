using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces;
public interface IBaseRepository<T> : IDisposable where T : class
{
    Task Create(T entity);
    Task<T> Read(string id);
    Task Update(T entity);
    Task Delete(string id);
}
