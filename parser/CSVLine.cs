using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    class CSVLine
    {
        public string Country { get; set; }
        public string Origin { get; set; }        
        public String Type { get; set; }
        public Int32?[] Value { get; }
        public string CountryIRI { get; set; }
        public string OriginIRI { get; set; }

        public CSVLine(string country, string origin, String type, Int32?[] value)
        {
            Country = country;
            Origin = origin;
            Type = type;
            Value = value;
        }
    }
}
