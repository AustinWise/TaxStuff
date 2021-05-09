using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using TaxTest.ExpressionEvaluation;

namespace TaxTest.FormModel
{
    class TaxYearDefinition
    {
        public TaxYearDefinition(string folderPath)
        {
            var forms = new Dictionary<string, FormDefinition>();
            foreach (var path in Directory.GetFiles(folderPath, "*.xml"))
            {
                try
                {
                    var doc = XDocument.Load(path, LoadOptions.SetLineInfo);
                    switch (doc.Root.Name.LocalName)
                    {
                        case "Form":
                            var form = new FormDefinition(Path.GetFileNameWithoutExtension(path), doc);
                            forms.Add(form.Name, form);
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
            this.Forms = new(forms);

            TypeCheck();
        }

        public ReadOnlyDictionary<string, FormDefinition> Forms { get; }

        private void TypeCheck()
        {
            foreach (var f in Forms.Values)
            {
                var env = new TypecheckEnvironment()
                {
                    Forms = Forms,
                    CurrentForm = f,
                };

                foreach (var line in f.Lines.Values)
                {
                    if (line.Calc is object)
                    {
                        var actualLineType = line.Calc.CheckType(env);
                        if (line.Type != actualLineType)
                        {
                            throw new TypecheckException($"Form{f.Name}.Line{line.Number} ({line.Name}) expected type {line.Type}, actual {actualLineType}.");
                        }
                    }
                }
            }
        }
    }
}
