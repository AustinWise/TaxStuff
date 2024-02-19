namespace TaxStuff.ExpressionEvaluation;

record class IfExpression(BaseExpression ConditionalExpression, BaseExpression TrueExpression, BaseExpression FalseExpression) : BaseExpression
{
    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        ConditionalExpression.ValidateExpressionType(env, BoolType.Instance);
        var trueType = TrueExpression.CheckType(env);
        var falseType = TrueExpression.CheckType(env);
        if (!trueType.Equals(falseType))
        {
            throw new TypecheckException($"The true and false sides of the IF expression are not the same types. True: {trueType} False: {falseType}");
        }
        return trueType;
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        var result = (BoolResult)ConditionalExpression.Evaluate(env);
        if (result.Value)
            return TrueExpression.Evaluate(env);
        else
            return FalseExpression.Evaluate(env);
    }
}
