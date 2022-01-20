using System;
using System.IO;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.FormModel;

namespace TaxStuff
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

            if (!File.Exists(returnPath))
            {
                throw new Exception("Tax return file does not exist: " + returnPath);
            }
            if (!Directory.Exists(outputFolder))
            {
                throw new Exception("Output directory does not exist: " + returnPath);
            }

            var taxUniverse = new TaxUniverse(Path.GetDirectoryName(typeof(Program).Assembly.Location));
            var taxReturn2020 = new TaxReturn(returnPath, taxUniverse);
            taxReturn2020.Calculate();

            //TODO: a better way of reporting
            foreach (var formKvp in taxReturn2020.Forms)
            {
                Console.WriteLine(formKvp.Key);
                foreach (var form in formKvp.Value.Forms)
                {
                    Console.WriteLine("\tForm");
                    var formValues = form.GetValueSnapshot();
                    foreach (var lineDef in form.Definition.Lines.Values)
                    {
                        if (!formValues.TryGetValue(lineDef.Name, out EvaluationResult value))
                            continue;

                        Console.Write("\t\t");
                        if (lineDef.Number is not null)
                            Console.Write($"Line: {lineDef.Number} ");
                        Console.Write($"Name: {lineDef.Name}");

                        if (value is ArrayResult array)
                        {
                            Console.WriteLine(" Values:");
                            foreach (var v in array.Values)
                            {
                                Console.WriteLine($"\t\t\t{v}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($" Value: {value}");
                        }

                    }
                }
            }

            foreach (var pdfForm in taxReturn2020.TaxYearDef.PdfInfo.Forms.Values)
            {
                if (taxReturn2020.Forms.TryGetValue(pdfForm.FormName, out FormInstances formInsts))
                {
                    int i = 1;
                    foreach (var inst in formInsts.Forms)
                    {
                        string outputFileName = formInsts.Forms.Count == 1 ? pdfForm.FormName + ".pdf" : $"{pdfForm.FormName}-{i}.pdf";
                        string outputPath = Path.Combine(outputFolder, outputFileName);

                        pdfForm.Save(outputPath, inst);

                        i++;
                    }
                }
            }
        }
    }
}
