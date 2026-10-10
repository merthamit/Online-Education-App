using System.Text.Json.Serialization;
using System;
namespace OnlineEdu.Entity.Entities
{
    public class BlogComment
    {
        public int BlogCommentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public int BlogId { get; set; }
        [JsonIgnore] public virtual Blog? Blog { get; set; }
    }
}