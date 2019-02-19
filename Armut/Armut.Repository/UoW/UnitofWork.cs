using Armut.Common.Result;
using Armut.DataAccess;
using Armut.Entity.Tables;
using Armut.Repository.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Armut.Repository.UoW
{
    public sealed class UnitofWork : IUnitofWork
    {
        private readonly PostgreDbContext _context;

        //private bool _disposed;
        public UnitofWork(PostgreDbContext context)
        {
            //Database initialize kapatıyorum.
            //Database.SetInitializer<AtomContext>(null);
            _context = context ?? throw new ArgumentException("context is null");
        }

        public IGenericRepository<T> GetRepository<T>() where T : class, new()
        {
            return new GenericRepository<T>(_context);
        }
        public ServiceResult SaveChanges()
        {

            //using (var transaction = _context.Database.BeginTransaction())
            //{
            var transaction = _context.Database.BeginTransaction();
            try
            {
                foreach (var dbEntityEntry in _context.ChangeTracker.Entries<BaseEntities>().Where(x => x.State == EntityState.Modified).ToList())
                {
                    try
                    {
                        dbEntityEntry.Property<DateTime>("CreatedDate").IsModified = false;
                        dbEntityEntry.Property<int>("CreatedUserId").IsModified = false;
                    }
                    catch (Exception ex)
                    {
                        //Ignored
                    }

                }

                //Encrypt Attribute'üne sahip propertyleri encrypt eder
                //foreach (var entity in _context.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Deleted).ToList())
                //{
                //    var propertyInfos = entity.Entity.GetType().GetProperties()
                //        .Where(x =>
                //            x.GetCustomAttributes(true).Any(attr => attr.GetType() == typeof(EncryptAttribute)) &&
                //            x.PropertyType == typeof(string)
                //        );

                //    foreach (var prop in propertyInfos)
                //    {
                //        prop.SetValue(entity.Entity, prop.GetValue(entity.Entity, null)?.ToString().Encrypt());
                //    }
                //}

                var affectedRow = _context.SaveChanges();
                transaction.Commit();
                return Result.ReturnAsSuccess(null, affectedRow);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Update the values of the entity that failed to save from the store 
                ex.Entries.Single().Reload();
                //return Result.ReturnAsFail(AlertResource.ConcurrencyControl);
                return Result.ReturnAsFail("");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result.ReturnAsFail();
                // }
            }
        }

        //public void Dispose(bool disposing)
        //{
        //    if (!this._disposed)
        //    {
        //        if (disposing)
        //        {
        //            _context.Dispose();
        //        }
        //    }
        //    this._disposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
