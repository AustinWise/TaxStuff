using System;
using System.Diagnostics;
using System.IO;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.FormModel;

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

var sw = Stopwatch.StartNew();
var taxUniverse = new TaxUniverse(Path.GetDirectoryName(AppContext.BaseDirectory)!);
long parsingDefs = sw.ElapsedMilliseconds;
var taxReturn = new TaxReturn(returnPath, taxUniverse);
long parsingReturn = sw.ElapsedMilliseconds;
taxReturn.Calculate();
long calculating = sw.ElapsedMilliseconds;
sw.Stop();

//TODO: a better way of reporting
foreach (var formKvp in taxReturn.Forms)
{
    Console.WriteLine(formKvp.Key);
    foreach (var form in formKvp.Value.Forms)
    {
        Console.Write("\tForm");
        if (!string.IsNullOrEmpty(form.SSN))
        {
            Console.Write($" (SSN: {form.SSN})");
        }
        Console.WriteLine();

        var formValues = form.GetValueSnapshot();
        foreach (var lineDef in form.Definition.Lines.Values)
        {
            if (!formValues.TryGetValue(lineDef.Name, out EvaluationResult? value))
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

foreach (var pdfForm in taxReturn.TaxYearDef.PdfInfo.Forms.Values)
{
    if (taxReturn.Forms.TryGetValue(pdfForm.FormName, out FormInstances? formInsts))
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

Console.WriteLine();
Console.WriteLine("Timings:");
Console.WriteLine($"\tLoading defs: {parsingDefs} ms");
Console.WriteLine($"\tLoading return: {parsingReturn - parsingDefs} ms");
Console.WriteLine($"\tcalculating: {calculating - parsingReturn} ms");