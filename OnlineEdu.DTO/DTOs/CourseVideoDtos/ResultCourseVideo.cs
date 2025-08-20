using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.DTO.DTOs.CourseVideoDtos
{
    public class ResultCourseVideo
    {
        public int CourseVideoId { get; set; }
        public int VideoNumber { get; set; }
        public int CourseId { get; set; }
        public ResultCourseVideo Course { get; set; }
        public string VideoUrl { get; set; }
    }
}
