namespace TaxStuff.ExpressionEvaluation
{
    interface IHasFieldTypes
    {
        ExpressionType GetFieldType(TypecheckEnvironment env, string fieldName);
    }
}
