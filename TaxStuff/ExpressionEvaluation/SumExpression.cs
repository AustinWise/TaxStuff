using System.Linq;

namespace TaxStuff.ExpressionEvaluation;

record SumExpression(BaseExpression Expression) : BaseExpression
{
    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        return EvaluationResult.CreateNumber(((ArrayResult)Expression.Evaluate(env)).Values.Sum(v => v.AsNumber()));
    }

    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        Expression.ValidateExpressionType(env, NumberType.ArrayInstance);
        return NumberType.Instance;
    }
}
