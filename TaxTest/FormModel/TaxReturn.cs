using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TaxTest.FormModel
{
    class TaxReturn
    {
        public int Year { get; }
        public FilingStatus Status { get; }
        public Dictionary<string, List<FormInstance>> Forms { get; }
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
                        List<FormInstance> formList;
                        if (Forms.TryGetValue(formInst.Name, out formList))
                        {
                            if (!formInst.Definition.AllowMultiple)
                                throw new FileLoadException(node, "Duplicate form: " + formInst.Name);
                        }
                        else
                        {
                            formList = new List<FormInstance>();
                            Forms.Add(formInst.Name, formList);
                        }
                        formList.Add(formInst);
                        break;
                    default:
                        throw new FileLoadException(node, "Unkown node name: " + node.Name);
                }
            }
        }

        public void Calculate()
        {
            foreach (var form in Forms.Values.SelectMany(f => f))
            {
                if (!form.Definition.Calculateable)
                    continue;

                var env = new ExpressionEvaluation.EvaluationEnvironment()
                {
                    Return = this,
                    CurrentFormName = form.Name,
                };

                foreach (var line in form.Definition.Lines.Values)
                {
                    if (!form.Values.ContainsKey(line.Name))
                    {
                        if (line.Calc is null)
                        {
                            form.Values.Add(line.Name, new List<decimal>() { 0m });
                        }
                        else
                        {
                            form.Values.Add(line.Name, new List<decimal>() { line.Calc.Evaluate(env).Number });
                        }
                    }
                }
            }
        }
    }
}
