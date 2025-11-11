using System;

namespace HeroServer
{
    public class GenValuesParams
    {
        public String TableName { get; set; }
        public String[] Columns { get; set; }
        public String Status { get; set; }
        public int FilterStatus { get; set; }
        public String[] FilterColumns { get; set; }
        public String[] FilterValues { get; set; }

        public GenValuesParams()
        {
        }

        public GenValuesParams(String tableName, String[] columns, String status, int filterStatus, String[] filterColumns, String[] filterValues)
        {
            TableName = tableName;
            Columns = columns;
            Status = status;
            FilterStatus = filterStatus;
            FilterColumns = filterColumns;
            FilterValues = filterValues;
        }
    }
}
