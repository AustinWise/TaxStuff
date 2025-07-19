using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxStuff.ExpressionEvaluation;

record class SelectSameSsnFormsExpression(BaseExpression FormExpression) : BaseExpression
{
    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        var formExprType = FormExpression.CheckType(env);
        if (formExprType is not ArrayType arrayType || arrayType.ElementType is not FormType)
        {
            string errorMessage = $"In Expression '{FormExpression}' is type '{formExprType}', expected a form array type.";
            throw new TypecheckException(errorMessage);
        }
        return formExprType;
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(env.CurrentForm);
        if (string.IsNullOrWhiteSpace(env.CurrentForm.SSN))
        {
            throw new InvalidOperationException($"An instance of {env.CurrentForm.Definition.Name} is missing the SSN attribute.");
        }

        var forms = (ArrayResult)FormExpression.Evaluate(env);
        var ret = new List<FormResult>();
        foreach (FormResult f in forms.Values.Cast<FormResult>())
        {
            if (string.IsNullOrWhiteSpace(f.Value.SSN))
            {
                throw new InvalidOperationException($"An instance of {f.Def.Name} is missing the SSN attribute.");
            }
            if (env.CurrentForm.SSN.Equals(f.Value.SSN, StringComparison.CurrentCultureIgnoreCase))
            {
                ret.Add(f);
            }
        }
        return new ArrayResult(ret);
    }
}
