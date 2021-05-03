using System;
using TaxTest.ExpressionParsing;
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
                    foreach (var line in form.Values)
                    {
                        var lineDef = form.Definition.Lines[line.Key];
                        if (line.Value.Count == 0)
                            throw new Exception("wut");
                        else if (line.Value.Count == 1)
                            Console.WriteLine($"\t\tLine: {lineDef.Number} Name: {line.Key} Value: {line.Value[0]:c}");
                        else
                        {
                            Console.WriteLine($"\t\tLine: {lineDef.Number} Name: {line.Key}");
                            foreach (var v in line.Value)
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
