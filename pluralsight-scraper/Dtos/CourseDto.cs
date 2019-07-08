namespace VH.PluralsightScraper.Dtos
{
    internal class CourseDto
    {
        public string CourseName { get; set; }
        public string CourseLevel { get; set; }
        public string CourseDate { get; set; }

        public static CourseDto Create(string courseName, string courseLevel, string courseDate)
        {
            return new CourseDto
                   {
                       CourseName = courseName,
                       CourseLevel = courseLevel,
                       CourseDate = courseDate
                   };
        }
    }
}
