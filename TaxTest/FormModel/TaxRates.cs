using System;

namespace TaxTest.FormModel
{
    class TaxRates
    {
        readonly TaxComputationWorksheet mWorksheet;

        public TaxRates(TaxComputationWorksheet worksheet)
        {
            mWorksheet = worksheet ?? throw new ArgumentNullException(nameof(worksheet));
        }

        public decimal CalculateTax(FilingStatus status, decimal taxableAmount)
        {
            var section = mWorksheet.GetSection(status);

            TaxComputationWorksheetLine theLine = null;
            foreach (var l in section.Lines)
            {
                if (taxableAmount >= l.Min)
                {
                    theLine = l;
                }
                else
                {
                    break;
                }
            }

            if (theLine is null)
                throw new NotImplementedException("Income too small to use Tax Computation Worksheet, Tax Tables not yet implemented.");

            return (taxableAmount * theLine.MultiplicationAmount) - theLine.SubtractionAmount;
        }
    }
}
