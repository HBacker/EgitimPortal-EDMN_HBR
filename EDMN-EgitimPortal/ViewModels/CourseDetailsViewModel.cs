// ViewModels/CourseLessonModel.cs
using EgitimPortalFinal.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.ViewModels
{

    public class CourseDetailsViewModel
    {
        public CourseModel Course { get; set; }
        public IEnumerable<CourseLessonModel> Lessons { get; set; } 
        public int CurrentLessonId { get; set; } 
        public string Section { get; set; }
    }

}
