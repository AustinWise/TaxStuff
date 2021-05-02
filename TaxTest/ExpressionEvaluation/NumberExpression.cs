namespace TaxTest.ExpressionEvaluation
{
    record NumberExpression(decimal Number) : BaseExpression
    {
        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return EvaluationResult.CreateNumber(Number);
        }

        public override ExpressionType GetType(TypecheckEnvironment env)
        {
            return NumberType.Instance;
        }
    }
}
