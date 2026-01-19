using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.FormModel;

var taxUniverse = new TaxUniverse(Path.GetDirectoryName(AppContext.BaseDirectory)!);

RootCommand rootCommand = new("TaxStuff command line tool");

#if DEBUG
var typeCheckCommand = new Command("type-check", "Type check all tax years.");
typeCheckCommand.SetAction(async _ =>
{
    var typeErrors = taxUniverse.TypeCheckAllYears();
    if (typeErrors.Count == 0)
    {
        Console.WriteLine("No type errors found.");
    }
    else
    {
        Console.WriteLine("Type errors found:");
        foreach (var kvp in typeErrors.OrderBy(k => k.Key))
        {
            Console.WriteLine();
            Console.WriteLine($"Year {kvp.Key}:");
            Console.WriteLine();
            Console.WriteLine(kvp.Value);
        }
    }
});
rootCommand.Subcommands.Add(typeCheckCommand);
#endif // DEBUG

var inputFileArgument = new Argument<FileInfo>("input")
{
    Description = "Path to the tax return XML file.",
};
var outputFolderOption = new Option<DirectoryInfo>("--output", "-o")
{
    Description = "Path to the output folder for generated PDF files.",
};

var calcCommand = new Command("calc", "Calculate a tax return.");
calcCommand.Arguments.Add(inputFileArgument);
calcCommand.Options.Add(outputFolderOption);
rootCommand.Subcommands.Add(calcCommand);

calcCommand.SetAction(parseResult =>
{
    var returnFileInfo = parseResult.GetRequiredValue(inputFileArgument);
    if (!returnFileInfo.Exists)
    {
        Console.WriteLine("Tax return file does not exist: " + returnFileInfo.FullName);
        Environment.Exit(1);
    }
    var returnPath = returnFileInfo.FullName;

    DirectoryInfo? outputFolderInfo = parseResult.GetValue(outputFolderOption);
    if (outputFolderInfo is not null && !outputFolderInfo.Exists)
    {
        Console.WriteLine("Output directory does not exist: " + outputFolderInfo.FullName);
        Environment.Exit(1);
    }

    var taxReturn = new TaxReturn(returnPath, taxUniverse);
    taxReturn.Calculate();

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

    if (outputFolderInfo is null)
    {
        return;
    }

    foreach (var pdfForm in taxReturn.TaxYearDef.PdfInfo.Forms.Values)
    {
        if (taxReturn.Forms.TryGetValue(pdfForm.FormName, out FormInstances? formInsts))
        {
            int i = 1;
            foreach (var inst in formInsts.Forms)
            {
                string outputFileName = formInsts.Forms.Count == 1 ? pdfForm.FormName + ".pdf" : $"{pdfForm.FormName}-{i}.pdf";
                string outputPath = Path.Combine(outputFolderInfo.FullName, outputFileName);

                pdfForm.Save(outputPath, inst);

                i++;
            }
        }
    }

});

var topLevelParseResult = rootCommand.Parse(args);

if (topLevelParseResult.Errors.Count != 0)
{
    Console.WriteLine("Errors parsing command line:");
    foreach (var error in topLevelParseResult.Errors)
    {
        Console.WriteLine(error.Message);
    }
    return 1;
}
else
{
    topLevelParseResult.Invoke();
    return 0;
}
