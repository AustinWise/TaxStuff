#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;

namespace TaxStuff.FormModel
{
    class TaxYearDefinition
    {
        public TaxYearDefinition(int year, string folderPath)
        {
            TaxComputationWorksheet? taxComputationWorksheet = null;
            PdfInfo? pdfInfo = null;
            var forms = new Dictionary<string, FormDefinition>();
            foreach (var path in Directory.GetFiles(folderPath, "*.xml"))
            {
                try
                {
                    var doc = XDocument.Load(path, LoadOptions.SetLineInfo);
                    if (doc.Root is null)
                        throw new Exception("Missing root element.");
                    switch (doc.Root.Name.LocalName)
                    {
                        case "Form":
                            var form = new FormDefinition(Path.GetFileNameWithoutExtension(path), doc);
                            forms.Add(form.Name, form);
                            break;
                        case "TaxComputationWorksheet":
                            if (taxComputationWorksheet is not null)
                                throw new Exception("Duplicate TaxComputationWorksheet");
                            taxComputationWorksheet = new TaxComputationWorksheet(doc);
                            break;
                        case "PdfInfo":
                            if (pdfInfo is not null)
                                throw new Exception("Duplicate PdfInfo");
                            pdfInfo = new PdfInfo(folderPath, doc);
                            break;
                        default:
                            throw new FileLoadException(doc.Root, "Unexpected document type: " + doc.Root.Name);
                    }
                }
                catch (Exception ex)
                {
                    throw new FileLoadException("Failed to load from " + path, ex);
                }
            }

            if (taxComputationWorksheet is null)
            {
                throw new Exception($"For tax year {year}, there is no TaxComputationWorksheet.");
            }

            this.Year = year;
            this.Forms = new(forms);
            this.Rates = new TaxRates(taxComputationWorksheet);
            this.PdfInfo = pdfInfo;

            TypeCheck();
        }

        public int Year { get; }
        public ReadOnlyDictionary<string, FormDefinition> Forms { get; }
        public TaxRates Rates { get; }
        public PdfInfo? PdfInfo { get; }

        private void TypeCheck()
        {
            foreach (var f in Forms.Values)
            {
                foreach (var line in f.Lines.Values)
                {
                    if (line.Calc is object)
                    {
                        var env = new TypecheckEnvironment(Forms, f, line);
                        var actualLineType = line.Calc.CheckType(env);
                        if (line.Type != actualLineType)
                        {
                            throw new TypecheckException($"Form{f.Name}.Line{line.Number} ({line.Name}) expected type {line.Type}, actual {actualLineType}.");
                        }
                    }
                }
                foreach (var assert in f.Asserts)
                {
                    var env = new TypecheckEnvironment(Forms, f);
                    var actualType = assert.Expr.CheckType(env);
                    if (actualType is not BoolType)
                    {
                        throw new TypecheckException($"Form{f.Name} assert expression '{assert.ExprStr}' should be a boolean, but the type is actually '{actualType}'.");
                    }
                }
            }
        }
    }
}
