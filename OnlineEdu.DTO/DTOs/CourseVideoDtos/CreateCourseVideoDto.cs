﻿using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.DTO.DTOs.CourseVideoDtos
{
    public class CreateCourseVideoDto
    {
        public int VideoNumber { get; set; }
        public int CourseId { get; set; }
        public string VideoUrl { get; set; }
    }
}
