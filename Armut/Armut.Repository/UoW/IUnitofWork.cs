using Armut.Common.Result;
using Armut.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armut.Repository.UoW
{
    public interface IUnitofWork //: IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class, new();
        ServiceResult SaveChanges();
    }
}
