namespace TaxStuff.ExpressionEvaluation
{
    interface IHasFieldEvaluation
    {
        EvaluationResult EvaluateField(EvaluationEnvironment env, string fieldName);
    }
}
