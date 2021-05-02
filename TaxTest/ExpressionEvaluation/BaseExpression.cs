namespace TaxTest.ExpressionEvaluation
{
    abstract record BaseExpression
    {
        public abstract EvaluationResult Evaluate(EvaluationEnvironment env);

        public abstract ExpressionType GetType(TypecheckEnvironment env);

        internal void ValidateExpressionType(TypecheckEnvironment env, ExpressionType expectedType)
        {
            var actualType = GetType(env);
            if (actualType != expectedType)
            {
                string errorMessage = $"In Expression '{this}' is type '{actualType}', expected '{expectedType}'.";
                throw new TypecheckException(errorMessage);
            }
        }
    }
}
