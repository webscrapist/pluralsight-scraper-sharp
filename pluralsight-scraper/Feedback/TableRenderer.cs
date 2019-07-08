using System;
using System.Linq;
using VH.PluralsightScraper.Domain;

namespace VH.PluralsightScraper.Feedback
{
    internal class TableRenderer
    {
        public TableRenderer(TableColumn[] columnsConfig)
        {
            _columnsConfig = columnsConfig ?? throw new ArgumentNullException(nameof(columnsConfig));
        }

        public string Headers
        {
            get
            {
                string columnNames = _columnsConfig.Select(c => c.Name.PadRight(c.Length)).Aggregate(AggregateFunction);
                string underlines = _columnsConfig.Select(c => new string('-', c.Length)).Aggregate(AggregateFunction);
                
                return $"{columnNames}\r\n{underlines}";
            }
        }

        public string DataRow(ReplicateResultDetail detail, int index)
        {
            return _columnsConfig.Select(column =>
                                         {
                                             string value = column.GetValue(detail, index);

                                             string paddedValue = column.Alignment == "right"
                                                                      ? value.PadLeft(column.Length)
                                                                      : value.PadRight(column.Length);

                                             return paddedValue;
                                         })
                                 .Aggregate(AggregateFunction);
        }
        
        private static string AggregateFunction(string left, string right) => $"{left}  {right}";

        private readonly TableColumn[] _columnsConfig;

        public const string COLUMN_NAME_INDEX = "index";
        public const string COLUMN_NAME_CHANNEL_NAME = "channel name";
        public const string COLUMN_NAME_ACTION = "action";
    }
}
