using Microsoft.EntityFrameworkCore;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.DataAccess.Context;
using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.DataAccess.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly OnlineEduContext _context;

        public GenericRepository(OnlineEduContext context)
        {
            _context = context;
        }
        public DbSet<T> Table { get => _context.Set<T>(); }
        public int Count()
        {
            return Table.Count();
        }

        public void Create(T entity)
        {
            Table.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = Table.Find(id);
            if (entity == null) return;
            Table.Remove(entity);
            _context.SaveChanges();
        }

        public int FilteredCount(Expression<Func<T, bool>> predicate)
        {
            return Table.Where(predicate).Count();
        }

        public T GetByFilter(Expression<Func<T, bool>> predicate)
        {
            return Table.Where(predicate).FirstOrDefault();
        }

        public T GetById(int id)
        {
            return Table.Find(id);
        }

        public List<T> GetFilteredList(Expression<Func<T, bool>> predicate)
        {
            return Table.Where(predicate).ToList();
        }

        public List<T> GetList()
        {
            return Table.ToList();
        }

        public void Update(T entity)
        {
            // Aynı anahtarla daha önce okunmuş (takip edilen) bir kayıt varsa onu bırak; aksi halde EF "already tracked" hatası verir
            var key = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
            if (key != null)
            {
                var keyValues = key.Properties.Select(p => p.PropertyInfo?.GetValue(entity)).ToArray();
                var tracked = _context.ChangeTracker.Entries<T>().FirstOrDefault(e =>
                    !ReferenceEquals(e.Entity, entity) &&
                    key.Properties.Select(p => e.Property(p.Name).CurrentValue).SequenceEqual(keyValues));
                if (tracked != null)
                {
                    tracked.State = EntityState.Detached;
                }
            }
            Table.Update(entity);
            _context.SaveChanges();
        }
    }
}
