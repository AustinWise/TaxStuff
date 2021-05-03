using System.Collections.ObjectModel;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    class TypecheckEnvironment
    {
        public ReadOnlyDictionary<string, FormDefinition> Forms { get; set; }
    }
}
