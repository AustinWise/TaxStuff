using System.Collections.Generic;
using TaxStuff.FormModel;

namespace TaxStuff.DataImport
{
    class Form1099Div : IConvertibleToFormInstance
    {
        public string PayerNameAndAddress { get; set; }
        public decimal TotalOrdinaryDividends { get; set; }
        public decimal QualifiedDividends { get; set; }
        public decimal NondividendDistributions { get; set; }
        public decimal FederalIncomeTaxWithheld { get; set; }
        public decimal Section199ADividends { get; set; }
        public decimal ForeignTaxPaid { get; set; }
        public decimal ExemptInterestDividends { get; set; }

        public FormInstance ConvertToFormInstance(TaxYearDefinition taxYear)
        {
            var formDef = taxYear.Forms["1099-DIV"];
            var numberValues = new Dictionary<string, decimal>()
            {
                { nameof(TotalOrdinaryDividends), TotalOrdinaryDividends },
                { nameof(QualifiedDividends), QualifiedDividends },
                { nameof(NondividendDistributions), NondividendDistributions },
                { nameof(FederalIncomeTaxWithheld), FederalIncomeTaxWithheld },
                { nameof(Section199ADividends), Section199ADividends },
                { nameof(ForeignTaxPaid), ForeignTaxPaid },
                { nameof(ExemptInterestDividends), ExemptInterestDividends },
            };
            var stringValues = new Dictionary<string, string>()
            {
                { nameof(PayerNameAndAddress), PayerNameAndAddress },
            };
            var formInst = new FormInstance(formDef, numberValues, stringValues);
            return formInst;
        }
    }
}
