﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TaxStuff.DataImport;

namespace TaxStuff.FormModel
{
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

            this.Year = doc.Root.IntAttributeValue("Year");
            this.Status = doc.Root.EnumAttributeValue<FilingStatus>("FilingStatus");
            this.Forms = new();

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
                            var dataImporter = DataImporterFactory.Create(el.Name.LocalName, el.AttributeValue("File"));
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
            FormInstances formList;
            if (!Forms.TryGetValue(formInst.Name, out formList))
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
}
