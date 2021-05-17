using System;
using System.Collections.Generic;
using TaxStuff.ExpressionEvaluation;

namespace TaxStuff.FormModel
{
    class FormInstances
    {
        public FormDefinition Definition { get; }
        public List<FormInstance> Forms { get; }

        public FormInstances(FormDefinition def)
        {
            this.Definition = def ?? throw new ArgumentNullException(nameof(def));
            this.Forms = new List<FormInstance>();
        }

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

            var env = new EvaluationEnvironment(@return, null);
            foreach (var inst in Forms)
            {
                inst.Calculate(env);
            }
        }
    }
}
