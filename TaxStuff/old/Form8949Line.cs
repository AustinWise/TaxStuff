using System;
using System.Xml.Linq;

namespace TaxStuff
{
    enum Form8949Code
    {
        /// <summary>
        /// Short-term transactions reported on Form(s) 1099-B showing basis was reported to the IRS
        /// </summary>
        A,
        /// <summary>
        /// Short-term transactions reported on Form(s) 1099-B showing basis wasn’t reported to the IRS
        /// </summary>
        B,
        /// <summary>
        ///  Short-term transactions not reported to you on Form 1099-B
        /// </summary>
        C,

        /// <summary>
        /// Long-term transactions reported on Form(s) 1099-B showing basis was reported to the IRS
        /// </summary>
        D,
        /// <summary>
        /// Long-term transactions reported on Form(s) 1099-B showing basis wasn’t reported to the IRS
        /// </summary>
        E,
        /// <summary>
        /// Long-term transactions not reported to you on Form 1099-B
        /// </summary>
        F,
    }

    /// <summary>
    /// Intended to load data from a schwab 1099-B export.
    /// </summary>
    class Form8949Line
    {
        public Form8949Line(XElement el)
        {
            this.Code = Enum.Parse<Form8949Code>(el.Element("FORM8949CODE").Value);
            throw new NotImplementedException();
        }

        public Form8949Code Code { get; }
        public DateTime? Acquired { get; }
        public DateTime Sold { get; }
        public string Description { get; }
        public decimal CostBasis { get; }
        public decimal SalePrice { get; }
        public string SecName { get; }
        public double NumberOfShares { get; }
    }
}
