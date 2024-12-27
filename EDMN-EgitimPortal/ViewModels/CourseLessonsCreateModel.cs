using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.ViewModels
{
    public class CourseLessonsCreateModel
    {
        public int CourseId { get; set; }
        public List<CourseLessonModel> Lessons { get; set; } = new List<CourseLessonModel>();

    }
}
