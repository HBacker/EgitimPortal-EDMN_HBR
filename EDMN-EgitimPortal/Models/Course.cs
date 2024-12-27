using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
        public string? PhotoUrl { get; set; } 
        public List<CourseLesson> Lessons { get; set; }
    }

}
