using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace parser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2 || !File.Exists(args[0]))
            {
                Console.WriteLine("Input incorrect");
                return;
            }

            //Read CSV
            var csvLines = CSVParser.parseCSV(args[0]);

            //The country that I could not find in DBpedia or that are listed under a different name
            Dictionary<String, String> missingMatched = new Dictionary<string, string>()
            {
                {"http://vocab.informatik.tuwien.ac.at/VU184.729-2018/1225159/Botswana", "Botswana" },
                {"http://dbpedia.org/resource/Venezuela", "Bolivarian Republic of Venezuela"},
                {"http://dbpedia.org/resource/Brunei", "Brunei Darussalam"},
                {"http://dbpedia.org/resource/Republic_of_the_Congo", "Congo"},
                {"http://dbpedia.org/resource/Ivory_Coast", "Côte d'Ivoire"},
                {"http://dbpedia.org/resource/Socialist_Federal_Republic_of_Yugoslavia", "Federal Republic of Yugoslavia"},
                {"http://dbpedia.org/resource/The_Gambia", "Gambia"},
                {"http://dbpedia.org/resource/Georgia_(country)", "Georgia"},
                {"http://dbpedia.org/resource/Republic_of_Ireland", "Ireland"},
                {"http://dbpedia.org/resource/Iran", "Islamic Republic of Iran"},
                {"http://dbpedia.org/resource/Laos", "Lao People's Democratic Republic"},
                {"http://dbpedia.org/resource/Libya", "Libyan Arab Jamahiriya"},
                {"http://dbpedia.org/resource/Palistin", "Occupied Palestinian Territory"},
                {"http://dbpedia.org/resource/Bolivia", "Plurinational State of Bolivia"},
                {"http://dbpedia.org/resource/South_Korea", "Republic of Korea"},
                {"http://dbpedia.org/resource/The_Bahamas", "Bahamas"},
                {"http://dbpedia.org/resource/Moldova", "Republic of Moldova"},
                {"http://dbpedia.org/resource/Russia", "Russian Federation"},
                {"http://vocab.informatik.tuwien.ac.at/VU184.729-2018/1225159/Sao_Tome_and_Pricipe", "Sao Tome and Principe"},
                {"http://dbpedia.org/resource/Serbia", "Serbia (and Kosovo: S/RES/1244 (1999))"},
                {"http://dbpedia.org/resource/Sint_Maarten", "Sint Maarten (Dutch part)"},
                {"http://dbpedia.org/resource/Syria", "Syrian Arab Republic"},
                {"http://dbpedia.org/resource/Socialist_Republic_of_Macedonia", "The former Yugoslav Republic of Macedonia"},
                {"http://dbpedia.org/resource/East_Timor", "Timor-Leste"},
                {"http://dbpedia.org/resource/Uganda_(Commonwealth_realm)", "Uganda"},
                {"http://dbpedia.org/resource/Tanzania", "United Republic of Tanzania"},
                {"http://vocab.informatik.tuwien.ac.at/VU184.729-2018/1225159/Botswana/VariousCountry", "Various"},
                {"http://dbpedia.org/resource/Vietnam", "Viet Nam"}
            };

            //SPARQL query to fetch country IRIs from DBpedia
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            SparqlParameterizedString query = new SparqlParameterizedString();
            query.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            query.CommandText = "SELECT ?country ?name WHERE { ?country a dbo:Country. ?country rdfs:label ?name FILTER (lang(?name) = 'en')} ORDER BY $name";
            SparqlResultSet results = endpoint.QueryWithResultSet(query.ToString());
            Dictionary<string, string> countryIRIs = results.ToDictionary(y => { var s = y.Value("name").ToString(); return s.Substring(0, s.Length - 3); }, y => y.Value("country").ToString());

            //Add the missing IRIs
            foreach (var missingCountry in missingMatched)
                countryIRIs.Add(missingCountry.Value, missingCountry.Key);

            //Set the country IRIs
            int i = 0;
            int position = 0;
            var iris = countryIRIs.OrderBy(y => y.Key).ToArray();
            foreach (var country in csvLines.OrderBy(y => y.Country))
            {
                do
                {
                    var c = iris[i];

                    position = String.Compare(country.Country, c.Key);

                    if (position == 0)
                        country.CountryIRI = c.Value;
                    else
                    {
                        if (position > 0)
                        {
                            i++;
                            if (i == iris.Length)
                                break;
                        }
                    }
                }
                while (position > 0);

                if (i == iris.Length)
                    break;
            }

            //Set the origin IRIs
            i = 0;
            foreach (var country in csvLines.OrderBy(y => y.Origin))
            {
                do
                {
                    var c = iris[i];

                    position = String.Compare(country.Origin, c.Key);

                    if (position == 0)
                        country.OriginIRI = c.Value;
                    else
                    {
                        if (position > 0)
                        {
                            i++;
                            if (i == iris.Length)
                                break;
                        }
                    }
                }
                while (position > 0);

                if (i == iris.Length)
                    break;
            }

            //Example output
            //@prefix dbo: <http://dbpedia.org/ontology/> .
            //@prefix dbp: <http://dbpedia.org/property/> .            
            //<http://dbpedia.org/resource/Venezuela> dbp:population[dbp:origin <http://dbpedia.org/resource/Venezuela>; dbp:status "Stateless"; dbp:year 2003; dbo:populationTotal 5542].
            
            using (StreamWriter sw = new StreamWriter(new FileStream(args[1], FileMode.Create)))
            {
                sw.WriteLine("@prefix dbo: <http://dbpedia.org/ontology/> .");
                sw.WriteLine("@prefix dbp: <http://dbpedia.org/property/> .");

                foreach(var line in csvLines)                
                    for (i = 0; i < 14; i++)                
                        if(line.Value[i] != null)
                            sw.WriteLine("<{0}> dbp:population[dbp:origin <{1}>; dbp:status \"{2}\"; dbp:year {3}; dbo:populationTotal {4}].", line.CountryIRI, line.OriginIRI, line.Type, 2000 + i, line.Value[i].ToString());                                
            }           
        }
    }
}
