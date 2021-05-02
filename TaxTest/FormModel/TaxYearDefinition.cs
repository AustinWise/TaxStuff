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
            var forms = new List<FormDefinition>();
            foreach (var f in Directory.GetFiles(folderPath, "*.xml"))
            {
                forms.Add(FormDefinition.LoadFromFile(f));
            }
            this.Forms = new ReadOnlyCollection<FormDefinition>(forms);
        }

        public ReadOnlyCollection<FormDefinition> Forms { get; }

        public void TypeCheck()
        {
            var formMap = new Dictionary<string, FormDefinition>();
            foreach (var f in Forms)
            {
                formMap.Add(f.Name, f);
            }
            var env = new TypecheckEnvironment()
            {
                Forms = formMap,
            };
            foreach (var f in Forms)
            {
                foreach (var line in f.Lines.Values)
                {
                }
            }
        }
    }
}
