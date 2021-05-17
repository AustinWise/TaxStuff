using System;

namespace TaxStuff.ExpressionEvaluation
{
    record MaxExpression(BaseExpression Left, BaseExpression Right) : BaseExpression
    {
        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            Left.ValidateExpressionType(env, NumberType.Instance);
            Right.ValidateExpressionType(env, NumberType.Instance);
            return NumberType.Instance;
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return EvaluationResult.CreateNumber(Math.Max(Left.Evaluate(env).AsNumber(), Right.Evaluate(env).AsNumber()));
        }
    }
}
