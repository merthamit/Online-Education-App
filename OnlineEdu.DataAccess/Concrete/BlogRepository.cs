using Microsoft.EntityFrameworkCore;
using OnlineEdu.DataAccess.Abstract;
using OnlineEdu.DataAccess.Context;
using OnlineEdu.DataAccess.Repositories;
using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.DataAccess.Concrete
{
    public class BlogRepository : GenericRepository<Blog>, IBlogRepository
    {
        private readonly OnlineEduContext _educontext;
        public BlogRepository(OnlineEduContext _context) : base(_context)
        {
            _educontext = _context;
        }

        public List<Blog> GetBlogsWithCategories()
        {
            return _educontext.Blogs.Include(x => x.BlogCategory).Include(x => x.Writer).ToList();
        }

        public List<Blog> GetBlogByWriterId(int id)
        {
            return _educontext.Blogs.Include(x => x.BlogCategory).Where(x => x.WriterId == id).ToList();
        }

        public List<Blog> GetLast4BlogsWithCategories()
        {
            return _educontext.Blogs.Include(x => x.BlogCategory).OrderByDescending(x => x.BlogDate).Take(4).ToList();
        }

        public Blog GetBlogWithCategories(int id)
        {
            return _educontext.Blogs.Include(x => x.BlogCategory).Include(x => x.Writer).ThenInclude(x => x.TeacherSocials).FirstOrDefault(x => x.BlogId == id);
        }

        public List<Blog> GetBlogsWithCategoriesByCategoryId(int id)
        {
            return _educontext.Blogs.Include(x => x.BlogCategory).Include(x => x.Writer).Where(x => x.BlogCategoryId == id).ToList();
        }
    }
}
