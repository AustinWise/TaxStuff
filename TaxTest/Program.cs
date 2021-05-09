using System;
using System.Collections.Generic;
using System.IO;
using TaxTest.FormModel;

namespace TaxTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Requires two arguments: input.xml outputFolder");
                return;
            }

            string returnPath = args[0];
            string outputFolder = args[1];

            var taxUniverse = new TaxUniverse(Path.GetDirectoryName(typeof(Program).Assembly.Location));
            var taxReturn2020 = new TaxReturn(returnPath, taxUniverse);
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

            foreach (var pdfForm in taxReturn2020.TaxYearDef.PdfInfo.Forms.Values)
            {
                if (taxReturn2020.Forms.TryGetValue(pdfForm.FormName, out List<FormInstance> formInsts))
                {
                    int i = 1;
                    foreach (var inst in formInsts)
                    {
                        string outputFileName = formInsts.Count == 1 ? pdfForm.FormName + ".pdf" : $"{pdfForm.FormName}-{i}.pdf";
                        string outputPath = Path.Combine(outputFolder, outputFileName);

                        pdfForm.Save(outputPath, inst);

                        i++;
                    }
                }
            }
        }
    }
}
