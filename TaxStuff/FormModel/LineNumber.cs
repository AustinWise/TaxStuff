using System;
using System.Text.RegularExpressions;

namespace TaxStuff.FormModel
{
    readonly struct LineNumber : IEquatable<LineNumber>, IComparable<LineNumber>
    {
        static readonly Regex sLineFormat = new Regex(@"^(?<number>\d+)(?<letter>\w)?$", RegexOptions.ExplicitCapture);

        readonly int _number;
        readonly char _letter;

        public LineNumber(string lineNumber)
        {
            var m = sLineFormat.Match(lineNumber);
            if (!m.Success)
                throw new ArgumentException("Invalid line number: " + lineNumber);
            this._number = int.Parse(m.Groups["number"].Value);
            var letterGroup = m.Groups["letter"];
            if (letterGroup.Success)
                this._letter = letterGroup.Value[0];
            else
                this._letter = default;
        }

        public int CompareTo(LineNumber other)
        {
            int ret = this._number.CompareTo(other._number);
            if (ret != 0)
                return ret;
            return this._letter.CompareTo(other._letter);
        }

        public bool Equals(LineNumber other)
        {
            return this._number == other._number &&
                   this._letter == other._letter;
        }

        public override string ToString()
        {
            if (this._letter == default)
                return this._number.ToString();
            else
                return $"{_number}{_letter}";
        }
    }
}
