using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TaxStuff.ExpressionEvaluation
{
    static class FunctionFactory
    {
        record FunctionInfo(int ArgCount, ConstructorInfo Ctor)
        {
        }

        static readonly Dictionary<string, FunctionInfo> sFunctions = new(StringComparer.OrdinalIgnoreCase);

        static void AddFunction<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]T>() where T : BaseExpression
        {
            const string EXPRESSION = "Expression";

            var type = typeof(T);
            if (!type.Name.EndsWith(EXPRESSION))
                throw new Exception($"Type {type.Name}'s name does not end with '{EXPRESSION}'");
            var ctors = type.GetConstructors();
            if (ctors.Length != 1)
                throw new Exception("too many ctors on " + type.Name);
            var ctor = ctors[0];

            foreach (var p in ctor.GetParameters())
            {
                if (p.ParameterType != typeof(BaseExpression))
                    throw new Exception($"wrong type for parameter '{p.Name}' on constructor for '{type.Name}'.");
            }

            sFunctions.Add(type.Name.Substring(0, type.Name.Length - EXPRESSION.Length), new FunctionInfo(ctor.GetParameters().Length, ctor));
        }

        static FunctionFactory()
        {
            AddFunction<IfExpression>();
            AddFunction<SumExpression>();
            AddFunction<MinExpression>();
            AddFunction<MaxExpression>();
            AddFunction<TaxExpression>();
            AddFunction<FilterForm8949Expression>();
            AddFunction<RoundToZeroIfBelowExpression>();
            AddFunction<SelectSameSsnFormsExpression>();
        }

        public static BaseExpression CreateFunction(string functionName, List<BaseExpression> arguments)
        {
            if (!sFunctions.TryGetValue(functionName, out FunctionInfo funcInfo))
            {
                throw new Exception("Unknown function '" + functionName + "'.");
            }
            if (funcInfo.ArgCount != arguments.Count)
            {
                throw new Exception($"Expected {funcInfo.ArgCount} argument for {functionName}, found {arguments.Count}");
            }
            return (BaseExpression)funcInfo.Ctor.Invoke(arguments.ToArray());
        }
    }
}
