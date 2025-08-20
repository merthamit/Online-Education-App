using Microsoft.EntityFrameworkCore;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.DataAccess.Context;
using OnlineEdu.DataAccess.Repositories;
using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.DataAccess.Concrete
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(OnlineEduContext context) : base(context)
        {
        }

        public void DontShowOnHome(int id)
        {
            var value = _context.Courses.Find(id);
            value.IsShown = false;
            _context.SaveChanges();
        }

        public List<Course> GetCourseByTeacherId(int id)
        {
            var result = _context.Courses.Include(x => x.Category).Where(x => x.AppUserId == id).ToList();
            return result;
        }

        public List<Course> GetCoursesWithCategories()
        {
            var result = _context.Courses.Include(x => x.Category).Include(x => x.AppUser).ToList();
            return result;
        }

        public List<Course> GetCoursesWithCategories(Expression<Func<Course, bool>> filter = null)
        {
            IQueryable<Course> values = _context.Courses.Include(x => x.Category).Include(x => x.AppUser).AsQueryable();
            if (filter != null)
            {
                values = values.Where(filter);
            }
            return values.ToList();
        }

        public void ShowOnHome(int id)
        {
            var value = _context.Courses.Find(id);
            value.IsShown = true;
            _context.SaveChanges();
        }

    }
}
