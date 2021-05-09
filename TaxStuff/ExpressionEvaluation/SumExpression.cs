using System.Linq;

namespace TaxStuff.ExpressionEvaluation
{
    record SumExpression(BaseExpression Expression) : BaseExpression
    {
        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return EvaluationResult.CreateNumber(Expression.Evaluate(env).Array.Sum());
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            Expression.ValidateExpressionType(env, NumberType.ArrayInstance);
            return NumberType.Instance;
        }
    }
}
