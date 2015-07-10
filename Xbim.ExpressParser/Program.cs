using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You have to specify input express schema file.");
                Console.ReadKey();
                return;
            }

            var path = args[0];
            if (!File.Exists(path))
            {
                Console.WriteLine("You have to specify existing express schema file.");
                Console.ReadKey();
                return;
            }

            using (var file = File.OpenRead(path))
            {
                var scanner = new Scanner(file);
                var parser = new Parser(scanner, IfcVersionEnum.IFC2x3);
                var result = parser.Parse();

                if (parser.Output != null)
                    parser.Output.Close();

                if (scanner.Errors.Any())
                {
                    if (!result) 
                    {
                        foreach (var err in scanner.Errors)
                        {
                            Console.WriteLine(err);
                        }
                        Console.WriteLine("Errors occured during the processing. Output might be incomplete or eroneous.");
                    }

                    else
                        Console.WriteLine("Errors occured during the processing but all of them had been catched.");
                }
                else
                    Console.WriteLine("Completed with no errors.");

                file.Close();
            }

            
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }
    }
}
