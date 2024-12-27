using EgitimPortalFinal.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.ViewModels
{
    public class CourseModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Kurs adı 100 karakterden uzun olamaz.")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Tag { get; set; }

        public string? PhotoUrl { get; set; }

        [Display(Name = "Kurs Fotoğrafı")]
        public IFormFile? PhotoFile { get; set; }

        public List<CourseLessonModel> Lessons { get; set; } = new List<CourseLessonModel>();
    }

}