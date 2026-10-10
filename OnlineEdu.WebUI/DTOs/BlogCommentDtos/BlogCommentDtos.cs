using System;
namespace OnlineEdu.WebUI.DTOs.BlogCommentDtos
{
    public class CreateBlogCommentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
    }
    public class ResultBlogCommentDto
    {
        public int BlogCommentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public int BlogId { get; set; }
    }
}