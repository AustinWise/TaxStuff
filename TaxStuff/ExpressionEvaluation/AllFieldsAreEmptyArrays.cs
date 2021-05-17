using System.Collections.Generic;

namespace TaxStuff.ExpressionEvaluation
{
    class AllFieldsAreEmptyArrays : IHasFieldEvaluation
    {
        public static readonly AllFieldsAreEmptyArrays Instance = new AllFieldsAreEmptyArrays();

        private AllFieldsAreEmptyArrays()
        {
        }

        public EvaluationResult EvaluateField(EvaluationEnvironment env, string fieldName)
        {
            return new ArrayResult(new List<EvaluationResult>());
        }
    }
}
