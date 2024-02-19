using System.Collections.Generic;
using TaxStuff.FormModel;

namespace TaxStuff.DataImport
{
    class Form1099Int : IConvertibleToFormInstance
    {
        public string? PayerNameAndAddress { get; set; }
        public decimal InterestIncome { get; set; }
        public decimal InterestOnUsSavingsBondsAndTreasuryObligations { get; set; }
        public decimal TaxExemptInterest { get; set; }

        public FormInstance ConvertToFormInstance(TaxYearDefinition taxYear)
        {
            var formDef = taxYear.Forms["1099-INT"];
            var numberValues = new Dictionary<string, decimal>()
            {
                { nameof(InterestIncome), InterestIncome },
                { nameof(InterestOnUsSavingsBondsAndTreasuryObligations), InterestOnUsSavingsBondsAndTreasuryObligations },
                { nameof(TaxExemptInterest), TaxExemptInterest },
            };
            var stringValues = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(PayerNameAndAddress))
            {
                stringValues.Add(nameof(PayerNameAndAddress), PayerNameAndAddress);
            }
            var formInst = new FormInstance(formDef, numberValues, stringValues);
            return formInst;
        }
    }
}
