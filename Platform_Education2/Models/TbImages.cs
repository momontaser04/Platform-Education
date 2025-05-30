using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PlatformEduPro.Models
{
    [Index(nameof(CourseId))]

    public class TbImages
    {
        [Key]
        public int Id { get; set; }
        public string ImagePath { get; set; }

        [ForeignKey("courses")]
        public int CourseId { get; set; }
        public virtual TbCourses courses { get; set; }

    }
}
