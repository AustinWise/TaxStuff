using System.Collections.ObjectModel;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation
{
    class TypecheckEnvironment
    {
        public ReadOnlyDictionary<string, FormDefinition> Forms { get; set; }
        public FormDefinition CurrentForm { get; set; }
    }
}
