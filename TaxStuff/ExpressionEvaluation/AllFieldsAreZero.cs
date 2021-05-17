namespace TaxStuff.ExpressionEvaluation
{
    class AllFieldsAreZero : IHasFieldEvaluation
    {
        public static readonly AllFieldsAreZero Instance = new AllFieldsAreZero();

        private AllFieldsAreZero()
        {
        }

        public EvaluationResult EvaluateField(EvaluationEnvironment env, string fieldName)
        {
            return EvaluationResult.Zero;
        }
    }
}
