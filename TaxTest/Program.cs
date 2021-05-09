using System;
using System.Collections.Generic;
using TaxTest.FormModel;

namespace TaxTest
{
    class Program
    {
        const string RETURN = @"d:\AustinWise\Desktop\Return.xml";
        const string OUTPUT = @"c:\temp\output.xml";
        static void Main(string[] args)
        {
            var taxUniverse = new TaxUniverse(@"C:\src\TaxTest\TaxTest\");
            var taxReturn2020 = new TaxReturn(RETURN, taxUniverse);
            taxReturn2020.Calculate();

            //TODO: a better way of reporting
            foreach (var formKvp in taxReturn2020.Forms)
            {
                Console.WriteLine(formKvp.Key);
                foreach (var form in formKvp.Value)
                {
                    Console.WriteLine("\tForm");
                    foreach (var lineDef in form.Definition.Lines.Values)
                    {
                        if (!form.Values.TryGetValue(lineDef.Name, out List<decimal> values))
                            continue;

                        if (values.Count == 0)
                            throw new Exception("wut");
                        else if (values.Count == 1)
                            Console.WriteLine($"\t\tLine: {lineDef.Number} Name: {lineDef.Name} Value: {values[0]:c}");
                        else
                        {
                            Console.WriteLine($"\t\tLine: {lineDef.Number} Name: {lineDef.Name}");
                            foreach (var v in values)
                            {
                                Console.WriteLine($"\t\t\t{v:c}");
                            }
                        }
                    }
                }
            }
        }
    }
}
