using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TaxTest.FormModel;

namespace TaxTest.ExpressionEvaluation
{
    record VariableExpression(string Form, string Line) : BaseExpression
    {
        //TODO: move this parsing into the parser itself
        private static readonly Regex sVariableRegex = new Regex(@"^(Form(?<form>[^.]+)\.)?(?<name>[^\.]+)$", RegexOptions.ExplicitCapture);

        private readonly string _originalExpression;

        public VariableExpression(string originalExpression)
            : this(null, null)
        {
            this._originalExpression = originalExpression;
            var m = sVariableRegex.Match(originalExpression);
            if (!m.Success)
                throw new Exception("Failed to parse expression: " + originalExpression);
            var formGroup = m.Groups["form"];
            if (formGroup.Success)
                this.Form = formGroup.Value;
            this.Line = m.Groups["name"].Value;
        }

        public override EvaluationResult Evaluate(EvaluationEnvironment env)
        {
            return env.GetValue(Form, Line);
        }

        public override ExpressionType CheckType(TypecheckEnvironment env)
        {
            // TODO: make throw a nicer error when the line does not exist
            FormDefinition form;
            if (Form == null)
            {
                form = env.CurrentForm;
            }
            else
            {
                try
                {
                    form = env.Forms[Form];
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception($"In expression {_originalExpression}, 'Form{Form}' does not exist.");
                }
            }

            LineDefinition line;
            try
            {
                if (Line.StartsWith("Line"))
                    line = form.LinesByNumber[Line.Substring(4)];
                else
                    line = form.Lines[Line];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception($"In expression {_originalExpression}, line '{Line}' does not exist.");
            }

            if (form.AllowMultiple || line.AllowMultiple)
            {
                return new ArrayType(line.Type);
            }
            else
            {
                return line.Type;
            }
        }
    }
}
