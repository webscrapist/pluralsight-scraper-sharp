namespace VH.PluralsightScraper.Domain
{
    internal class CoursesMissingData
    {
        public string CourseFieldName { get; }
        public double MissingPercentage { get; }
        public int MissingCount { get; }
        public int TotalCoursesCount { get; }

        public CoursesMissingData(string courseFieldName, int missingCount, int totalCoursesCount)
        {
            CourseFieldName = courseFieldName;
            MissingCount = missingCount;
            TotalCoursesCount = totalCoursesCount;

            if (totalCoursesCount == 0)
            {
                MissingPercentage = 0;
            }
            else
            {
                MissingPercentage = missingCount / (double) totalCoursesCount;
            }
        }
    }
}
