using System;
using System.Collections.Generic;
using System.Linq;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

record FormReferenceExpression(string FormName) : BaseExpression
{
    public override ExpressionType CheckType(TypecheckEnvironment env)
    {
        if (!env.Forms.TryGetValue(FormName, out FormDefinition? def))
        {
            throw new Exception("Unknown form name: " + FormName);
        }

        if (def.AllowMultiple && def != env.CurrentForm)
        {
            return new ArrayType(new FormType(def));
        }
        else
        {
            return new FormType(def);
        }
    }

    public override EvaluationResult Evaluate(EvaluationEnvironment env)
    {
        if (env.Return.Forms.TryGetValue(FormName, out FormInstances? form))
        {
            if (form.Definition.AllowMultiple)
            {
                if (env.CurrentForm.Definition == form.Definition)
                {
                    foreach (var f in form.Forms)
                    {
                        if (f == env.CurrentForm)
                        {
                            return new FormResult(form.Definition, f);
                        }
                    }
                    throw new Exception("Could not find current form.");
                }
                else
                {
                    return new ArrayResult(form.Forms.Select(f => new FormResult(form.Definition, f)));
                }
            }
            else
            {
                if (form.Forms.Count != 1)
                    throw new Exception($"Expected there to be exactly one Form{FormName}.");
                return new FormResult(form.Definition, form.Forms[0]);
            }
        }
        else
        {
            // Assume that since type check passed, it is a valid form name.
            // There are no instances attached to this return, so all values are
            // treated as zero.
            // We don't have to worry about treating references to the current form
            // as non-array, because we know that FormName does not exist in this
            // return and therefore cannot possiblely be the current form.
            var def = env.Return.TaxYearDef.Forms[FormName];
            if (def.AllowMultiple)
                return new ArrayResult([]);
            else
                return new FormResult(def, AllFieldsAreZero.Instance);
        }
    }
}
