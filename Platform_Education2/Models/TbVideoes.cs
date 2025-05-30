using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlatformEduPro.Models
{
    public class TbVideoes
    {
        [Key]
        public int Id { get; set; }
        public string VideoTitle { get; set; }
        public string VideoURL { get; set; }


        [ForeignKey("Section")]
        public int SectionId { get; set; }
        [JsonIgnore]
        public virtual TbSections Section { get; set; }
    }
}
