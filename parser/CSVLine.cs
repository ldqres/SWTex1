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
        public enum PopulationType { AsylumSeekers, InternallyDisplaced, OthersOfConcern, Refugees, ReturnedIDP, ReturnedRefugee, Stateless}
        public PopulationType type { get; set; }
        public Int32?[] value { get; }

        public CSVLine(string country, string origin, PopulationType type, Int32?[] value)
        {
            Country = country;
            Origin = origin;
            this.type = type;
            this.value = value;
        }
    }
}
