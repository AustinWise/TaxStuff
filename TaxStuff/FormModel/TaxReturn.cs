using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TaxStuff.DataImport;

namespace TaxStuff.FormModel;

class TaxReturn
{
    public int Year { get; }
    public FilingStatus Status { get; }
    public Dictionary<string, FormInstances> Forms { get; }
    public TaxYearDefinition TaxYearDef { get; }

    public TaxReturn(string filePath, TaxUniverse universe)
    {
        using var reader = new StreamReader(filePath);

        var doc = XDocument.Load(reader, LoadOptions.SetLineInfo);

        if (doc.Root is null)
        {
            throw new Exception("Missing root element in " + filePath);
        }

        this.Year = doc.Root.IntAttributeValue("Year");
        this.Status = doc.Root.EnumAttributeValue<FilingStatus>("FilingStatus");
        this.Forms = [];

        TaxYearDef = universe.TaxYears[Year];

        foreach (var node in doc.Root.Elements())
        {
            switch (node.Name.LocalName)
            {
                case "Form":
                    var formInst = new FormInstance(node, TaxYearDef);
                    AddForm(formInst);
                    break;
                case "DataImporters":
                    foreach (var el in node.Elements())
                    {
                        string importPath = el.AttributeValue("File");
                        if (!Path.IsPathFullyQualified(importPath))
                        {
                            importPath = Path.Combine(Path.GetDirectoryName(filePath)!, importPath);
                        }
                        var dataImporter = DataImporterFactory.Create(el.Name.LocalName, importPath);
                        foreach (var f in dataImporter.GetForms(TaxYearDef.Year))
                        {
                            AddForm(f.ConvertToFormInstance(TaxYearDef));
                        }
                    }
                    break;
                default:
                    throw new FileLoadException(node, "Unkown node name: " + node.Name);
            }
        }
    }

    public void AddForm(FormInstance formInst)
    {
        if (!Forms.TryGetValue(formInst.Name, out FormInstances? formList))
        {
            formList = new FormInstances(formInst.Definition);
            Forms.Add(formInst.Name, formList);
        }
        formList.AddForm(formInst);
    }

    public void Calculate()
    {
        foreach (var form in Forms.Values)
        {
            form.Calculate(this);
        }
    }
}
