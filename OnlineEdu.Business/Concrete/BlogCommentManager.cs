using OnlineEdu.Business.Abstract;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.Entity.Entities;
using System.Collections.Generic;

namespace OnlineEdu.Business.Concrete
{
    public class BlogCommentManager : IBlogCommentService
    {
        private readonly IBlogCommentRepository _repository;
        public BlogCommentManager(IBlogCommentRepository repository) { _repository = repository; }
        public void TDelete(int id) { _repository.Delete(id); }
        public BlogComment TGetById(int id) { return _repository.GetById(id); }
        public List<BlogComment> TGetList() { return _repository.GetList(); }
        public void TCreate(BlogComment entity) { _repository.Create(entity); }
        public void TUpdate(BlogComment entity) { _repository.Update(entity); }
        public int TCount() { return _repository.Count(); }
        public List<BlogComment> TGetFilteredList(System.Linq.Expressions.Expression<System.Func<BlogComment, bool>> predicate) { return _repository.GetFilteredList(predicate); }
        public BlogComment TGetByFilter(System.Linq.Expressions.Expression<System.Func<BlogComment, bool>> predicate) { return _repository.GetByFilter(predicate); }
        public int TFilteredCount(System.Linq.Expressions.Expression<System.Func<BlogComment, bool>> predicate) { return _repository.FilteredCount(predicate); }
    }
}