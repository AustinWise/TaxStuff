using System;

namespace TaxStuff.ExpressionEvaluation;

class AllFieldsAreZero : IHasFieldEvaluation
{
    public static readonly AllFieldsAreZero Instance = new();

    private AllFieldsAreZero()
    {
    }

    // If we don't throw here, we will throw in SelectSameSsnFormsExpression.
    // If we wait till then to throw, we will say a form is missing an attribute.
    // Instead we throw here to say the form is entirely missing.
    public string SSN => throw new InvalidOperationException("A reference was made to a form's SSN, but that form is not defined in the return.");

    public EvaluationResult EvaluateField(EvaluationEnvironment env, string fieldName)
    {
        return EvaluationResult.Zero;
    }
}
