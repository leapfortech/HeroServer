using System;

namespace HeroServer
{
    public class CatalogRecord
    {
        public long Id { get; set; }
        public String[] Fields { get; set; }
        public String[] Filters { get; set; }

        public CatalogRecord(long id, String[] fields, String[] filters)
        {
            Id = id;
            Fields = fields;
            Filters = filters;
        }
    }
}
