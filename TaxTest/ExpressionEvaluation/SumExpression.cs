using System.Linq;

namespace TaxTest.ExpressionEvaluation
{
    record SumExpression(BaseExpression Expression) : BaseExpression
    {
        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return EvaluationResult.CreateNumber(Expression.Evaluate(env).Array.Sum());
        }

        public override ExpressionType GetType(TypecheckEnvironment env)
        {
            Expression.ValidateExpressionType(env, NumberType.ArrayInstance);
            return NumberType.Instance;
        }
    }
}
