using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

record Form8949CodeReferenceExpression : BaseExpression
{
    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        return new EnumNameType(typeof(Form8949Code));
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        return new Form8949EnumNameResult();
    }
}
