using System;
using System.Collections.Generic;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    class EvaluationEnvironment
    {
        public TaxReturn Return { get; set; }
        public string CurrentFormName { get; set; }
        public TaxRates Rates => Return.TaxYearDef.Rates;

        public EvaluationResult GetValue(string form, string line)
        {
            if (form == null)
                form = CurrentFormName;

            var ret = new List<decimal>();
            bool multiple = false;
            if (Return.Forms.TryGetValue(form, out List<FormInstance> formInstances))
            {
                foreach (var f in formInstances)
                {
                    multiple |= f.Definition.AllowMultiple;
                    LineDefinition lineDef;
                    if (line.StartsWith("Line"))
                    {
                        if (f.Definition.LinesByNumber.TryGetValue(line.Substring(4), out lineDef))
                            line = lineDef.Name;
                    }
                    else
                    {
                        f.Definition.Lines.TryGetValue(line, out lineDef);
                    }
                    if (lineDef is null)
                    {
                        throw new Exception("Could not find line definition: " + line);
                    }
                    multiple |= lineDef.AllowMultiple;


                    if (f.Values.TryGetValue(line, out List<decimal> values))
                    {
                        ret.AddRange(values);
                    }
                    else if (lineDef.Calc is null)
                    {
                        ret.Add(0m);
                    }
                    else
                    {
                        values = new List<decimal>();
                        var result = lineDef.Calc.Evaluate(this);
                        ret.Add(result.Number);
                        values.Add(result.Number);

                        // This is the last; circular definition will cause stack overflows.
                        f.Values.Add(line, values);
                    }
                }
            }

            if (multiple)
            {
                return EvaluationResult.CreateArray(ret.ToArray());
            }
            else
            {
                switch (ret.Count)
                {
                    case 0:
                        return EvaluationResult.Zero;
                    case 1:
                        return EvaluationResult.CreateNumber(ret[0]);
                    default:
                        throw new Exception($"Unexpected count for Form{form}.{line}");
                }
            }
        }
    }
}
