namespace TaxStuff.ExpressionEvaluation
{
    record TaxExpression(BaseExpression Expr) : BaseExpression
    {
        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            Expr.ValidateExpressionType(env, NumberType.Instance);
            return NumberType.Instance;
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return EvaluationResult.CreateNumber(env.Rates.CalculateTax(env.Return.Status, Expr.Evaluate(env).Number));
        }
    }
}
