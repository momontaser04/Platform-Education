namespace PlatformEduPro.DTO.Section
{
    public class SectionDto
    {
        public int Id { get; set; }
        public string SectionName { get; set; }
        public int CourseId { get; set; }
        public List<string> videoTitle {  get; set; }

        public string ExamTitle { get; set; }
        public int ExamId { get; set; }

    }
}
