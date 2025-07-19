using System;
using System.Collections.Generic;
using TaxStuff.ExpressionEvaluation;

namespace TaxStuff.FormModel;

class FormInstances(FormDefinition def)
{
    public FormDefinition Definition { get; } = def ?? throw new ArgumentNullException(nameof(def));
    public List<FormInstance> Forms { get; } = [];

    public void AddForm(FormInstance form)
    {
        if (Forms.Count != 0 && !Definition.AllowMultiple)
            throw new InvalidOperationException($"Multiple Form{Definition.Name} are not allowed.");
        Forms.Add(form);
    }

    public void Calculate(TaxReturn @return)
    {
        if (!Definition.Calculateable)
            return;

        foreach (var inst in Forms)
        {
            inst.Calculate(@return);
        }
    }
}
