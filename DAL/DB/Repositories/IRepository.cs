using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IRepository<T>
    {
        public void Insert(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        IEnumerable<T> GetAll(); 
        public void SaveData();
        public T GetById(int id);
    }
}
