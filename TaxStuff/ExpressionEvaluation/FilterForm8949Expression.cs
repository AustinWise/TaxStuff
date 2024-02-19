using System.Collections.Generic;
using System.Diagnostics;

namespace TaxStuff.ExpressionEvaluation;

record FilterForm8949Expression(BaseExpression FormsExpr, BaseExpression CodeExpr) : BaseExpression
{
    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        var formArrayType = new ArrayType(new FormType(env.Forms["8949"]));
        FormsExpr.ValidateExpressionType(env, formArrayType);
        CodeExpr.ValidateExpressionType(env, EnumElementType.Form8949Code);
        return formArrayType;
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        var forms = (ArrayResult)FormsExpr.Evaluate(env);
        var code = (Form8949EnumElementResult)CodeExpr.Evaluate(env);
        var ret = new List<FormResult>();
        foreach (FormResult f in forms.Values)
        {
            Debug.Assert(f.Def.Name == "8949");
            var formCode = (Form8949EnumElementResult)f.GetFieldValue(env, "Code");
            if (formCode.Code == code.Code)
                ret.Add(f);
        }
        return new ArrayResult(ret);
    }
}
