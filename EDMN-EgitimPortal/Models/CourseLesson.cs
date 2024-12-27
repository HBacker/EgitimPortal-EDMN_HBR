using System.ComponentModel.DataAnnotations;

namespace EgitimPortalFinal.Models
{
    public class CourseLesson
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Ders adı 100 karakterden uzun olamaz.")]
        public string Name { get; set; }

        public string? Video { get; set; }

        public string? Thumbnail { get; set; }

        public bool Watched { get; set; } = false;

        [Required]
        public int OrderNo { get; set; } 

        public virtual Course Course { get; set; }
    }

}
