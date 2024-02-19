﻿using System;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

//TODO: identify similar constructs in tax law and generalize this concept. Via some sort of lambda support
//in the expression evaluator:
//    SUM(EvalPerTaxPayer(env => MAX(0, SUM(FormW-2.SocialSecurityTaxWithheld) - 8853.60)))
record class ExcessSocialSecurityTaxWithheldExpression(decimal MaxSocialSecurityTax) : BaseExpression
{
    public ExcessSocialSecurityTaxWithheldExpression(XElement node)
        : this(node.DecimalAttributeValue("MaxSocialSecurityTax"))
    {
    }

    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        return NumberType.Instance;
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        return new NumberResult(env.Return.Forms["W-2"].Forms.GroupBy(f => f.SSN)
                                                             .Select(person => Math.Max(0, person.Sum(f => f.EvaluateField(env, "SocialSecurityTaxWithheld").AsNumber()) - MaxSocialSecurityTax))
                                                             .Sum());
    }
}
