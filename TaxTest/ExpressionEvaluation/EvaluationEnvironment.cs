using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    class EvaluationEnvironment
    {
        public TaxReturn Return { get; set; }

        public EvaluationResult GetValue(string form, string line)
        {
            var ret = new List<decimal>();
            bool multiple = false;
            foreach (var f in Return.Forms[form])
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

            if (multiple)
            {
                return EvaluationResult.CreateArray(ret.ToArray());
            }
            else if (ret.Count == 1)
            {
                return EvaluationResult.CreateNumber(ret[0]);
            }
            else
            {
                throw new Exception($"Unexpected count for Form{form}.{line}");
            }
        }
    }
}
