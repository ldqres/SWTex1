using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parser
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 2 || !File.Exists(args[0]) || File.Exists(args[1]))
            {
                Console.WriteLine("Input incorrect");
                return;
            }

            //Read CSV
            var csvLines = CSVParser.parseCSV(args[0]);

            using (StreamWriter sw = new StreamWriter(new FileStream(args[1], FileMode.CreateNew)))
            {

                
            }
           

        }
    }
}
