using System;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper.Feedback
{
    internal class TableColumn
    {
        public string Name { get; }
        public int Length { get; }
        public string Alignment { get; }
        public Func<ReplicateResultDetail, int, string> GetValue { get; }

        public TableColumn(string name, 
                           int length, 
                           string alignment, 
                           Func<ReplicateResultDetail, int, string> getValue)
        {
            if (length < name.Length)
            {
                throw new Exception("length must be at least as long as the column name");
            }

            Name = name;
            Length = length;
            Alignment = alignment;
            GetValue = getValue;
        }
    }
}
