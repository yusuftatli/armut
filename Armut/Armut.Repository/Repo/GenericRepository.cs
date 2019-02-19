using Armut.DataAccess;
using Armut.Entity.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Armut.Repository.Repo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        private readonly PostgreDbContext _context;

        private readonly DbSet<T> _dbSet;
        public GenericRepository(PostgreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking();
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsNoTracking();
        }

        public T GetById(object id)
        {
            var entity = _dbSet.Find(id);

            var entty = entity as BaseEntities;
            if (entty != null && entty.IsDeleted)
            {
                return null;
            }

            return entity;
        }
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsNoTracking().FirstOrDefault();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);

            var baseEntities = entity as BaseEntities;
            if (baseEntities != null)
            {
                baseEntities.CreatedDate = DateTime.Now;
                baseEntities.CreatedUserId = 1;//CurrentUser.UserId();
            }
            if (entity.GetType().GetProperty("IsDeleted") != null)
            {
                T _entity = entity;

                _entity.GetType().GetProperty("IsDeleted")?.SetValue(_entity, false);
            }

        }
        public void AddRange(List<T> entityList)
        {
            _dbSet.AddRange(entityList);
            foreach (var item in entityList)
            {
                var baseEntities = item as BaseEntities;
                if (baseEntities != null)
                {
                    baseEntities.CreatedDate = DateTime.Now;
                    baseEntities.CreatedUserId = 1;//CurrentUser.UserId();
                    baseEntities.GetType().GetProperty("IsDeleted")?.SetValue(baseEntities, false);
                }
            }
        }
        public void Update(T entity)
        {
            //var tableName = GetTableName(entity);

            T originalEntity = null;

            //var auditList = new List<AuditingHistory>();

            var baseEntities = entity as BaseEntities;

            var state = _context.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            var sbColumns = new StringBuilder();

            //var columnList = _context.Database
            //    .SqlQuery<TableColumnDto>(
            //        $"SELECT COLUMN_NAME ColumnName FROM information_schema.columns WHERE TABLE_NAME = '{tableName}'").ToList();

            //foreach (var column in columnList)
            //{
            //    sbColumns.Append($"[{column.ColumnName}],");
            //}

            if (baseEntities != null)
            {
                baseEntities.UpdatedDate = DateTime.Now;
                baseEntities.UpdatedUserId = 1;//CurrentUser.UserId();
            }

            //if (baseEntities != null && columnList.Count != 0)
            //{
            //    originalEntity = GetRawSqlEntity<T>($"SELECT {sbColumns.ToString().Remove(sbColumns.Length - 1)} FROM {tableName} WHERE Id = {baseEntities.Id}");
            //}

            _context.Entry(entity).State = EntityState.Modified;


            if (originalEntity == null) return;

            var sbOldValues = new StringBuilder();
            var sbNewValues = new StringBuilder();

            var originalPropArray = originalEntity.GetType().GetProperties();
            var currentPropArray = entity.GetType().GetProperties();

            var stringLis = new List<string> { "CreatedDate", "CreatedUserId", "UpdatedDate", "UpdatedUserId" };

            foreach (var current in currentPropArray.Where(x => stringLis.Contains(x.Name).Equals(false)))
            {
                var original = originalPropArray.FirstOrDefault(x => x.Name == current.Name);


                if (original == null || current.Name != original.Name)
                    continue;

                if (original.GetValue(originalEntity)?.ToString() == current.GetValue(entity)?.ToString())
                    continue;
                if (original.PropertyType.FullName != null && original.PropertyType.FullName.Contains("Atom.Entities"))
                    continue;

                sbOldValues.Append($"{original.Name} : {original.GetValue(originalEntity)}, ");
                sbNewValues.Append($"{current.Name} : {current.GetValue(entity)}, ");
            }

            if (string.IsNullOrWhiteSpace(sbOldValues.ToString()) ||
                string.IsNullOrWhiteSpace(sbNewValues.ToString())) return;

            //var audit = new AuditingHistory
            //{
            //    TableName = entity.GetType().Name,
            //    OldValues = JsonConvert.SerializeObject(sbOldValues.ToString().Remove(sbOldValues.Length - 2)),
            //    NewValues = JsonConvert.SerializeObject(sbNewValues.ToString().Remove(sbNewValues.Length - 2)),
            //    KeyValues = baseEntities.Id.ToString(),
            //    UserId = CurrentUser.UserId(),
            //    DateTime = DateTime.Now
            //};

            //auditList.Add(audit);

            //_context.AuditingHistory.AddRange(auditList);
        }

        public void Delete(T entity)
        {
            T entityDelete = entity;

            var baseEntities = entity as BaseEntities;
            if (baseEntities != null)
            {
                baseEntities.DeletedDate = DateTime.Now;
                baseEntities.DeletedUserId = 1;//CurrentUser.UserId();
            }

            //Entity olarak referans verilmiş property'lerin içini boşaltır
            entity.GetType().GetProperties()
                .Where(z =>
                    (z.PropertyType.FullName ?? "").Contains("Entities")
                ).ToList()
                .ForEach(x => x.SetValue(entity, null));

            // IsDelete alanı olan tablolarda kayıt silinmez ve IsDelete alanı update edilir.
            if (entity.GetType().GetProperty("IsDeleted") != null)
            {
                entityDelete.GetType().GetProperty("IsDeleted")?.SetValue(entityDelete, true);

                Update(entityDelete);
            }
            else
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
            }
        }
    }
}
