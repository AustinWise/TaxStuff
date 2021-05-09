using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaxStuff.ExpressionEvaluation
{
    abstract record ExpressionType
    {
    }

    sealed record NumberType : ExpressionType
    {
        public static NumberType Instance { get; } = new NumberType();
        public static ArrayType ArrayInstance { get; } = new ArrayType(Instance);
    }

    sealed record EnumType(string Name, ReadOnlyCollection<string> Elements) : ExpressionType
    {
        public EnumType(string name, IList<string> elements)
            : this(name, new ReadOnlyCollection<string>(elements))
        {
        }
    }

    sealed record StructType(string Name, ReadOnlyDictionary<string, ExpressionType> Fields) : ExpressionType
    {
        public StructType(string name, IDictionary<string, ExpressionType> fields)
            : this(name, new ReadOnlyDictionary<string, ExpressionType>(fields))
        {
        }
    }

    sealed record ArrayType(ExpressionType ElementType) : ExpressionType
    {
    }
}