using Microsoft.Extensions.Caching.Distributed;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.Entity.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineEdu.Business.Concrete
{
    public class BlogManager : GenericManager<Blog>, IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IDistributedCache _cache;
        private const string AllBlogsKey = "blogs_with_categories";
        private const string Last4Key = "blogs_last4";
        private static readonly JsonSerializerOptions JsonOptions = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
        private static readonly DistributedCacheEntryOptions CacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        public BlogManager(IRepository<Blog> _repository, IBlogRepository blogRepository, IDistributedCache cache) : base(_repository)
        {
            _blogRepository = blogRepository;
            _cache = cache;
        }

        // Blog listeleri sık okunur; Redis'ten servis edilir, değişiklikte önbellek temizlenir
        private List<Blog> GetOrSet(string key, Func<List<Blog>> factory)
        {
            var cached = _cache.GetString(key);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<Blog>>(cached, JsonOptions) ?? new List<Blog>();
            }
            var values = factory();
            _cache.SetString(key, JsonSerializer.Serialize(values, JsonOptions), CacheOptions);
            return values;
        }

        private void InvalidateCache()
        {
            _cache.Remove(AllBlogsKey);
            _cache.Remove(Last4Key);
        }

        public override void TCreate(Blog entity)
        {
            base.TCreate(entity);
            InvalidateCache();
        }

        public override void TUpdate(Blog entity)
        {
            base.TUpdate(entity);
            InvalidateCache();
        }

        public override void TDelete(int id)
        {
            base.TDelete(id);
            InvalidateCache();
        }

        public List<Blog> TGetBlogByWriterId(int id)
        {
            return _blogRepository.GetBlogByWriterId(id);
        }

        public List<Blog> TGetBlogsWithCategories()
        {
            return GetOrSet(AllBlogsKey, () => _blogRepository.GetBlogsWithCategories());
        }

        public List<Blog> TGetBlogsWithCategoriesByCategoryId(int id)
        {
            return _blogRepository.GetBlogsWithCategoriesByCategoryId(id);
        }

        public Blog TGetBlogWithCategories(int id)
        {
            return _blogRepository.GetBlogWithCategories(id);
        }

        public List<Blog> TGetLast4BlogsWithCategories()
        {
            return GetOrSet(Last4Key, () => _blogRepository.GetLast4BlogsWithCategories());
        }
    }
}
