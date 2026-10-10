using Microsoft.Extensions.Caching.Distributed;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.Entity.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.Business.Concrete
{
    public class CourseCategoryManager : GenericManager<CourseCategory>, ICourseCategoryService
    {
        private readonly ICourseCategoryRepository _courseCategoryRepository;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "course_categories";

        public CourseCategoryManager(IRepository<CourseCategory> _repository, ICourseCategoryRepository courseCategoryRepository, IDistributedCache cache) : base(_repository)
        {
            _courseCategoryRepository = courseCategoryRepository;
            _cache = cache;
        }

        public override List<CourseCategory> TGetList()
        {
            var cachedCategories = _cache.GetString(CacheKey);
            if (!string.IsNullOrEmpty(cachedCategories))
            {
                return JsonSerializer.Deserialize<List<CourseCategory>>(cachedCategories);
            }

            var categories = base.TGetList();
            var jsonOptions = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };
            var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
            _cache.SetString(CacheKey, JsonSerializer.Serialize(categories, jsonOptions), cacheOptions);

            return categories;
        }

        public override void TCreate(CourseCategory entity)
        {
            base.TCreate(entity);
            _cache.Remove(CacheKey);
        }

        public override void TUpdate(CourseCategory entity)
        {
            base.TUpdate(entity);
            _cache.Remove(CacheKey);
        }

        public override void TDelete(int id)
        {
            base.TDelete(id);
            _cache.Remove(CacheKey);
        }

        public void TDontShowOnHome(int id)
        {
            _courseCategoryRepository.DontShowOnHome(id);
            _cache.Remove(CacheKey);
        }

        public void TShowOnHome(int id)
        {
            _courseCategoryRepository.ShowOnHome(id);
            _cache.Remove(CacheKey);
        }
    }
}
