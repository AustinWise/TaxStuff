using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
                var form = FormDefinition.LoadFromFile(path);
                forms.Add(form.Name, form);
            }
            this.Forms = new(forms);
            
            TypeCheck();
        }

        public ReadOnlyDictionary<string, FormDefinition> Forms { get; }

        private void TypeCheck()
        {
            var env = new TypecheckEnvironment()
            {
                Forms = Forms,
            };
            foreach (var f in Forms.Values)
            {
                foreach (var line in f.Lines.Values)
                {
                    line.Calc?.CheckType(env);
                }
            }
        }
    }
}
