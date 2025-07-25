﻿using iText.Forms.Fields;
using iText.Forms.Xfa;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;

namespace TaxStuff.FormModel;

class FormPdfInfo
{
    public string FormName { get; }
    readonly string _filePath;
    readonly Dictionary<string, string> _lineToFieldName = [];

    public FormPdfInfo(string dirPath, XElement node)
    {
        FormName = node.AttributeValue("Name");
        _filePath = Path.Combine(dirPath, node.AttributeValue("File"));

        using var reader = new PdfReader(_filePath);
        using var pdfdoc = new PdfDocument(reader);

        var pdfformn = iText.Forms.PdfAcroForm.GetAcroForm(pdfdoc, false);
        var allFieldNames = new List<string>();
        foreach (var kvp in pdfformn.GetFormFields())
        {
            allFieldNames.Add(kvp.Key);
        }

        var xfa = new XfaForm(pdfdoc);
        var xfaXml = xfa.GetDomDocument();

        var fieldsAndAssistName = new List<(string, string)>();
        XNamespace NS = "http://www.xfa.org/schema/xfa-template/3.0/";
        foreach (var field in xfaXml.Descendants(NS + "field"))
        {
            string? fieldName = field.Attribute("name")?.Value;
            string? assistName = field.Element(NS + "assist")?.Element(NS + "speak")?.Value;
            if (fieldName is not null && assistName is not null)
                fieldsAndAssistName.Add((fieldName, assistName));
        }

        foreach (var el in node.Elements("Line"))
        {
            string lineNumber = el.AttributeValue("Number");
            string expectedAssistText = el.AttributeValue("Assist");

            string fieldNameSubstring;
            var potentialFields = fieldsAndAssistName.Where(tup => tup.Item2.Contains(expectedAssistText)).ToList();
            if (potentialFields.Count == 1)
                fieldNameSubstring = potentialFields[0].Item1;
            else
                fieldNameSubstring = potentialFields.Where(f => f.Item2 == expectedAssistText).Single().Item1;

            var fieldFullName = allFieldNames.Where(name => name.Contains(fieldNameSubstring)).Single();
            _lineToFieldName.Add(lineNumber, fieldFullName);
        }
    }

    public void Save(string outputPath, FormInstance formInst)
    {
        using var reader = new PdfReader(_filePath);
        using var writer = new PdfWriter(outputPath);
        using var pdfdoc = new PdfDocument(reader, writer);

        var pdfformn = iText.Forms.PdfAcroForm.GetAcroForm(pdfdoc, false);

        var valueMap = formInst.GetValueSnapshot();

        foreach (var kvp in _lineToFieldName)
        {
            var lineDef = formInst.Definition.LinesByNumber[kvp.Key];
            var value = valueMap[lineDef.Name];
            var field = pdfformn.GetField(kvp.Value);
            field.SetValue(FormatResult(value));
        }

        static string FormatResult(EvaluationResult result)
        {
            return result switch
            {
                NumberResult num => num.Value.ToString("c"),
                StringResult str => str.Value,
                _ => throw new NotSupportedException(),
            };
        }
    }

    private static void SetCheckbox(PdfButtonFormField box)
    {
        // this worked on a 2020 f8949

        var checkState = box.GetAppearanceStates()[0];
        box.SetFieldFlag(32768, true);
        box.SetValue(checkState);
        box.Put(PdfName.AS, new PdfName(checkState));
    }
}
