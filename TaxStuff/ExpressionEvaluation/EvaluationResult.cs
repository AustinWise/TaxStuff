using System;

namespace TaxStuff.ExpressionEvaluation
{
    readonly struct EvaluationResult
    {
        public static readonly EvaluationResult Zero = new EvaluationResult(decimal.Zero, null);
        public static readonly EvaluationResult EmptyArray = new EvaluationResult(decimal.Zero, System.Array.Empty<decimal>());

        readonly decimal number;
        readonly object other;

        private EvaluationResult(decimal number, object other)
        {
            this.number = number;
            this.other = other;
        }

        public static EvaluationResult CreateNumber(decimal number)
        {
            return new EvaluationResult(number, null);
        }

        public static EvaluationResult CreateArray(decimal[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            return new EvaluationResult(decimal.Zero, array);
        }

        public decimal Number
        {
            get
            {
                if (other is object)
                    throw new InvalidOperationException($"This {nameof(EvaluationResult)} does not contain Money, it contains \"{other}\".");
                return number;
            }
        }

        public decimal[] Array
        {
            get
            {
                var ret = other as decimal[];
                if (ret is null)
                {
                    string errorMessage = other is null ? $"This value does not contain an array, it contains a number."
                                                        : $"This value does not contain an array, it contains \"{other}\".";
                    throw new InvalidOperationException(errorMessage);
                }
                return ret;
            }
        }

        public override string ToString()
        {
            if (other is null)
                return number.ToString();
            else
                return other.ToString();
        }
    }
}
