using System;

namespace VH.PluralsightScraper.Domain
{
    internal static class CourseLevelExtensions
    {
        public static CourseLevel ToEnum(this string rawValue)
        {
            return Enum.TryParse(rawValue, out CourseLevel courseLevel) 
                       ? courseLevel 
                       : CourseLevel.Unknown;
        }
    }
}
