using DAL.DB.Model;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DB.Repositories.Interfaces
{
    public interface IExtendedShuttleRepository : IRepository<Shuttle>
    {
        Shuttle GetByName(string name);
    }
}
