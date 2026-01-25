using System;

namespace TaxStuff.ExpressionEvaluation;

record FloorExpression(BaseExpression Expr) : BaseExpression
{
    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        Expr.ValidateExpressionType(env, NumberType.Instance);
        return NumberType.Instance;
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        return EvaluationResult.CreateNumber(Math.Floor(Expr.Evaluate(env).AsNumber()));
    }
}
