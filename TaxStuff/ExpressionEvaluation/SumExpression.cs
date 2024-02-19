using System.Linq;

namespace TaxStuff.ExpressionEvaluation;

record SumExpression(BaseExpression Expression) : BaseExpression
{
    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        return EvaluationResult.CreateNumber(((ArrayResult)Expression.Evaluate(env)).Values.Select(v => v.AsNumber()).Sum());
    }

    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        Expression.ValidateExpressionType(env, NumberType.ArrayInstance);
        return NumberType.Instance;
    }
}
