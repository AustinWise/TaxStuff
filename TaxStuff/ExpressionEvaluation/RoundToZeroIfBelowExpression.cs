namespace TaxStuff.ExpressionEvaluation
{
    internal record class RoundToZeroIfBelowExpression(BaseExpression MinValue, BaseExpression Expression) : BaseExpression
    {
        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            var min = MinValue.Evaluate(env).AsNumber();
            var value = Expression.Evaluate(env).AsNumber();
            return EvaluationResult.CreateNumber(value < min ? 0m : value);
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            MinValue.ValidateExpressionType(env, NumberType.Instance);
            Expression.ValidateExpressionType(env, NumberType.Instance);
            return NumberType.Instance;
        }
    }
}
