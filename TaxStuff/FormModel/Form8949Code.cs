namespace TaxStuff.FormModel
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
}
