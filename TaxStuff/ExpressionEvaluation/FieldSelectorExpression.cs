using System;

namespace TaxStuff.ExpressionEvaluation
{
    record FieldSelectorExpression(BaseExpression Source, string Selector) : BaseExpression
    {
        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            var sourceType = Source.CheckType(env);
            if (sourceType is ArrayType { ElementType: IHasFieldTypes elementFieldType })
            {
                return new ArrayType(elementFieldType.GetFieldType(env, Selector));
            }
            else if (sourceType is IHasFieldTypes fieldType)
            {
                return fieldType.GetFieldType(env, Selector);
            }
            else
            {
                throw new Exception($"Does not have fields: {Source} Type: {sourceType}");
            }
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return Source.Evaluate(env).GetFieldValue(env, Selector);
        }
    }
}
