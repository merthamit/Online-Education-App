using OnlineEdu.Business.Abstract;
using OnlineEdu.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.Business.Concrete
{
    public class GenericManager<T>(IRepository<T> _repository) : IGenericService<T> where T : class
    {
        public virtual int TCount()
        {
           return _repository.Count();
        }

        public virtual void TCreate(T entity)
        {
            _repository.Create(entity);
        }

        public virtual void TDelete(int id)
        {
            _repository.Delete(id);
        }

        public virtual int TFilteredCount(Expression<Func<T, bool>> predicate)
        {
            return _repository.FilteredCount(predicate);
        }

        public virtual T TGetByFilter(Expression<Func<T, bool>> predicate)
        {
            return _repository.GetByFilter(predicate);
        }

        public virtual T TGetById(int id)
        {
            return _repository.GetById(id);
        }

        public virtual List<T> TGetFilteredList(Expression<Func<T, bool>> predicate)
        {
            return _repository.GetFilteredList(predicate);
        }

        public virtual List<T> TGetList()
        {
            return _repository.GetList();
        }

        public virtual void TUpdate(T entity)
        {
            _repository.Update(entity);

        }
    }
}
