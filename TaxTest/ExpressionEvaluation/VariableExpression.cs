using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaxTest.ExpressionEvaluation
{
    record VariableExpression(string expression) : BaseExpression
    {
        private static readonly Regex sVariableRegex = new Regex(@"^Form(?<form>[^.]+)\.(?<name>[^\.])", RegexOptions.ExplicitCapture);

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            throw new NotImplementedException();
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            throw new NotImplementedException();
        }
    }
}
