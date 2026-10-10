using Microsoft.Extensions.Caching.Distributed;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineEdu.Business.Concrete
{
    public class CourseManager : GenericManager<Course>, ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IDistributedCache _cache;
        private const string CoursesWithCategoriesKey = "courses_with_categories";
        private const string CourseListKey = "course_list";
        private static readonly JsonSerializerOptions JsonOptions = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
        private static readonly DistributedCacheEntryOptions CacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        public CourseManager(IRepository<Course> _repository, ICourseRepository courseRepository, IDistributedCache cache) : base(_repository)
        {
            _courseRepository = courseRepository;
            _cache = cache;
        }

        private List<Course> GetOrSet(string key, Func<List<Course>> factory)
        {
            var cached = _cache.GetString(key);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<Course>>(cached, JsonOptions);
            }

            var values = factory();
            _cache.SetString(key, JsonSerializer.Serialize(values, JsonOptions), CacheOptions);
            return values;
        }

        private void InvalidateCache()
        {
            _cache.Remove(CoursesWithCategoriesKey);
            _cache.Remove(CourseListKey);
        }

        public override List<Course> TGetList()
        {
            return GetOrSet(CourseListKey, () => base.TGetList());
        }

        public override void TCreate(Course entity)
        {
            base.TCreate(entity);
            InvalidateCache();
        }

        public override void TUpdate(Course entity)
        {
            base.TUpdate(entity);
            InvalidateCache();
        }

        public override void TDelete(int id)
        {
            base.TDelete(id);
            InvalidateCache();
        }

        public void TDontShowOnHome(int id)
        {
            _courseRepository.DontShowOnHome(id);
            InvalidateCache();
        }

        public List<Course> TGetCourseByTeacherId(int id)
        {
            return _courseRepository.GetCourseByTeacherId(id);
        }

        public List<Course> TGetCoursesWithCategories()
        {
            return GetOrSet(CoursesWithCategoriesKey, () => _courseRepository.GetCoursesWithCategories());
        }

        public List<Course> TGetCoursesWithCategories(Expression<Func<Course, bool>> filter = null)
        {
            if (filter == null)
            {
                return TGetCoursesWithCategories();
            }
            return _courseRepository.GetCoursesWithCategories(filter);
        }

        public void TShowOnHome(int id)
        {
            _courseRepository.ShowOnHome(id);
            InvalidateCache();
        }
    }
}
