using System;

namespace VH.PluralsightScraper.Dtos
{
    internal class CourseDto
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string DatePublished { get; set; }

        public DateTime? DatePublishedParsed
        {
            get
            {
                if (DateTime.TryParse(DatePublished, out DateTime datePublishedParsed))
                {
                    return datePublishedParsed;
                }

                return null;
            }
        }

        public string ComparisonKey => CreateComparisonKey(Name, DatePublishedParsed);

        public static CourseDto Create(string name, string level, string datePublished)
        {
            return new CourseDto
                   {
                       Name = name,
                       Level = level,
                       DatePublished = datePublished
                   };
        }

        public static string CreateComparisonKey(string name, DateTime? datePublished)
        {
            return $"[{name.ToLower()}][{datePublished?.ToShortDateString()}]";
        }
    }
}
