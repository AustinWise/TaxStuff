#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace TaxStuff.FormModel
{
    class FormInstance : IHasFieldEvaluation
    {
        readonly Dictionary<string, EvaluationResult> _values = new();

        public FormDefinition Definition { get; }
        public string Name => Definition.Name;
        public string? SSN { get; }

        public FormInstance(FormDefinition def, Dictionary<string, decimal> numberValues, Dictionary<string, string> stringValues)
        {
            if (def is null)
                throw new ArgumentNullException(nameof(def));
            Definition = def;

            foreach (var kvp in numberValues)
            {
                addLineFromCtor(kvp.Key, NumberType.Instance, new NumberResult(kvp.Value));
            }
            foreach (var kvp in stringValues)
            {
                addLineFromCtor(kvp.Key, StringType.Instance, new StringResult(kvp.Value));
            }
        }

        public FormInstance(FormDefinition def, Form8949Code code, List<Form8949Line> transactions)
        {
            if (def is null)
                throw new ArgumentNullException(nameof(def));
            Definition = def;

            if (def.Name != "8949")
                throw new Exception("This constructor is form Form 8949 only.");

            addLineFromCtor("Code", EnumElementType.Form8949Code, new Form8949EnumElementResult(code));
            addLineFromCtor("Transactions", Form8949LineType.ArrayInstance, new ArrayResult(transactions.Select(t => new Form8949LineResult(t))));
        }

        void addLineFromCtor(string lineName, ExpressionType valueType, EvaluationResult value)
        {
            var lineDef = Definition.Lines[lineName];
            if (lineDef.Type != valueType)
                throw new Exception($"Form{Definition.Name}.{lineDef.Name} is should be of type {lineDef.Type}, recieved a {valueType}");
            _values.Add(lineName, value);
        }

        public FormInstance(XElement node, TaxYearDefinition taxYear)
        {
            var formName = node.AttributeValue("Name");
            this.Definition = taxYear.Forms[formName];
            this.SSN = node.OptionalAttributeValue("SSN");

            if (string.IsNullOrWhiteSpace(SSN) && Definition.RequireSsn)
            {
                throw new FileLoadException(node, "Missing SSN attribute.");
            }

            foreach (var el in node.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "Line":
                        foreach (var attr in el.Attributes())
                        {
                            switch (attr.Name.LocalName)
                            {
                                case "Number":
                                case "Name":
                                case "Value":
                                    break;
                                default:
                                    throw new FileLoadException(attr, "Unexpected attribute: " + attr.Name.LocalName);
                            }
                        }

                        string? number = el.OptionalAttributeValue("Number");
                        string? name = el.OptionalAttributeValue("Name");
                        LineDefinition lineDef;
                        if (number is null && name is null)
                            throw new FileLoadException(el, "Missing Name and Number attributes on line.");
                        else if (number is not null && name is not null)
                            throw new FileLoadException(el, "Missing Name and Number attributes on line.");
                        else if (name is not null)
                            lineDef = Definition.Lines[name]; // TODO: nicer exception for missing line
                        else
                        {
                            Debug.Assert(number is not null);
                            lineDef = Definition.LinesByNumber[number]; // TODO: nicer exception for missing line
                        }

                        _values.Add(lineDef.Name, ParseValue(lineDef, el));

                        break;
                    default:
                        throw new FileLoadException(el, "Unkown node name: " + el.Name);
                }
            }

            static EvaluationResult ParseValue(LineDefinition lineDef, XElement el)
            {
                var attr = el.Attribute("Value");
                if (attr is not null && lineDef.Type is StringType)
                    return new StringResult(attr.Value);

                try
                {
                    var parsedExpr = MyExpressionParser.Parse(new ParsingEnvironment(), el, "Value");
                    if (parsedExpr is null)
                        throw new FileLoadException(el, "Missing value for line.");
                    var actualType = parsedExpr.CheckType(new TypecheckEnvironment());
                    if (actualType != lineDef.Type)
                        throw new Exception($"Expected type {lineDef.Type}, found {actualType}.");
                    return parsedExpr.Evaluate(new EvaluationEnvironment(null, null));
                }
                catch (Exception ex)
                {
                    throw new FileLoadException(attr, "Failed to parse Value", ex);
                }
            }
        }

        public Dictionary<string, EvaluationResult> GetValueSnapshot()
        {
            return new Dictionary<string, EvaluationResult>(_values);
        }

        public void Calculate(EvaluationEnvironment env)
        {
            foreach (var line in Definition.Lines.Values)
            {
                if (!_values.ContainsKey(line.Name))
                {
                    EvaluateField(env, line.Name);
                }
            }
            foreach (var assert in Definition.Asserts)
            {
                var result = assert.Expr.Evaluate(env) as BoolResult;
                if (result is null)
                {
                    // PROGRAMMING ERROR: the type checker should have caught this
                    throw new Exception($"On form {Definition.Name}, assert expression '{assert.ExprStr}' did not evaluate to a boolean result.");
                }
                if (result.Value != assert.ExpectedValue)
                {
                    throw new Exception($"On form {Definition.Name}, assert expression '{assert.ExprStr}' should have evaluated to {assert.ExpectedValue} but was {result.Value}.");
                }
            }
        }

        public EvaluationResult EvaluateField(EvaluationEnvironment env, string fieldName)
        {
            LineDefinition lineDef;
            if (fieldName.StartsWith("Line"))
                lineDef = Definition.LinesByNumber[fieldName.Substring(4)];
            else
                lineDef = Definition.Lines[fieldName];

            if (_values.TryGetValue(lineDef.Name, out EvaluationResult? value))
            {
                return value;
            }

            if (lineDef.Calc is object)
                value = lineDef.Calc.Evaluate(env with { CurrentForm = this });
            else if (lineDef.Type == NumberType.Instance)
                value = EvaluationResult.CreateNumber(0m);
            else if (lineDef.Type is ArrayType)
                value = new ArrayResult(new List<EvaluationResult>());
            else
                throw new Exception($"Don't know how to make a default value for line {lineDef.Name} of type {lineDef.Type}.");

            _values.Add(lineDef.Name, value);
            return value;
        }
    }
}
