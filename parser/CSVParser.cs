using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    static class CSVParser
    {
        public static List<CSVLine> parseCSV(string path)
        {
            List<CSVLine> result = new List<CSVLine>();
            using (StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                for (int i = 0; i < 6; i++)
                    sr.ReadLine(); //Ignore the header lines

                while(!sr.EndOfStream)
                {
                    var splittedLine = sr.ReadLine().Split(',');
                    if(splittedLine.Length == 17) //Ignore malformed lines
                    {
                        //Read the values within the years
                        Int32?[] values = new Int32?[14];
                        Int32 parsedValue;
                        Int32? value;                        
                        for(int i = 3; i < 17; i++)
                        {
                            if (Int32.TryParse(splittedLine[i], out parsedValue))
                                value = parsedValue;
                            else
                                value = null;

                            values[i - 3] = value;
                        }

                        CSVLine.PopulationType type;
                        switch(splittedLine[2])
                        {
                            case "Internally displaced":
                                type = CSVLine.PopulationType.InternallyDisplaced;
                                break;
                            case "Returned IDPs":
                                type = CSVLine.PopulationType.ReturnedIDP;
                                break;                            
                            case "Returned refugees":
                                type = CSVLine.PopulationType.ReturnedRefugee;
                                break;
                            case "Asylum seekers":
                                type = CSVLine.PopulationType.AsylumSeekers;
                                break;
                            case "Refugees":
                                type = CSVLine.PopulationType.Refugees;
                                break;
                            case "Stateless":
                                type = CSVLine.PopulationType.Stateless;
                                break;
                            default:
                                type = CSVLine.PopulationType.OthersOfConcern;
                                break;
                        }
                        result.Add(new CSVLine(splittedLine[0], splittedLine[1], type, values));
                    }
                }
            }
            return result;
        }
    }
}
