using EgitimPortalFinal.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.ViewModels
{

    public class CourseLessonModel
    {
            public int Id { get; set; }

            [Required(ErrorMessage = "Kurs seçimi zorunludur")]
            public int CourseId { get; set; }

            [Required(ErrorMessage = "Ders adı zorunludur")]
            [Display(Name = "Ders Adı")]
            public string Name { get; set; }

            public string? Video { get; set; }
            public string? Thumbnail { get; set; }

            [Display(Name = "Video")]
            public IFormFile? VideoFile { get; set; }

            [Display(Name = "Thumbnail")]
            public IFormFile? ThumbnailFile { get; set; }

            public int OrderNo { get; set; }

            public CourseModel? Course { get; set; }
    }




    }
