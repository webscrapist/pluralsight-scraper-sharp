namespace VH.PluralsightScraper.Dtos
{
    internal class CourseDto
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string DatePublished { get; set; }

        public static CourseDto Create(string name, string level, string datePublished)
        {
            return new CourseDto
                   {
                       Name = name,
                       Level = level,
                       DatePublished = datePublished
                   };
        }
    }
}
