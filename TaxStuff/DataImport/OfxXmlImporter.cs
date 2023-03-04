using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TaxStuff.DataImport.OFX;
using TaxStuff.FormModel;

namespace TaxStuff.DataImport
{
    /// <summary>
    /// Mostly based on the files exported by Schwab, which are OFX 2.0.2.
    /// </summary>
    class OfxXmlImporter : IDataImporter
    {
        readonly string _filePath;

        public OfxXmlImporter(string filePath)
        {
            this._filePath = filePath;
        }

        class Myreader : XmlTextReader
        {
            public Myreader(TextReader reader)
                : base(reader)
            {
            }

            public override string NamespaceURI
            {
                get
                {
                    var ns = base.NamespaceURI;
                    if (string.IsNullOrEmpty(ns))
                    {
                        // Schwab does not set the namespace -_-
                        ns = "http://ofx.net/types/2003/04";
                    }
                    return ns;
                }
            }
        }

        public List<IConvertibleToFormInstance> GetForms(int taxYear)
        {
            var ret = new List<IConvertibleToFormInstance>();


            var ser = new XmlSerializer(typeof(OFX.OFX));
            OFX.OFX ofx;
            using (var tr = new StreamReader(_filePath))
            using (var reader = new Myreader(tr))
            {
                ofx = (OFX.OFX)ser.Deserialize(reader);
            }

            var transactions = new Dictionary<Form8949Code, List<Form8949Line>>()
            {
                {  Form8949Code.A, new List<Form8949Line>() },
                {  Form8949Code.B, new List<Form8949Line>() },
                {  Form8949Code.C, new List<Form8949Line>() },
                {  Form8949Code.D, new List<Form8949Line>() },
                {  Form8949Code.E, new List<Form8949Line>() },
                {  Form8949Code.F, new List<Form8949Line>() },
            };

            foreach (var item in ofx.Item.TAX1099TRNRS)
            {
                if (item.STATUS.CODE != "0" || item.STATUS.MESSAGE != "SUCCESS" || item.STATUS.SEVERITY != OFX.SeverityEnum.INFO)
                    throw new Exception($"Error import 1099 from {_filePath}: Code: {item.STATUS.CODE} Severity: {item.STATUS.SEVERITY} Message {item.STATUS.MESSAGE}");

                foreach (var f in item.TAX1099RS.Items)
                {
                    var genericForm = (AbstractTaxForm1099)f;
                    if (int.Parse(genericForm.TAXYEAR, CultureInfo.InvariantCulture) != taxYear)
                        throw new Exception($"Tax year does not match, expected {taxYear}, found {genericForm.TAXYEAR}.");

                    switch (f)
                    {


                        case Tax1099INT_V100 intForm:
                            ret.Add(new Form1099Int()
                            {
                                PayerNameAndAddress = intForm.PAYERADDR.PAYERNAME1,
                                InterestIncome = ParseMoney(intForm.INTINCOME),
                                TaxExemptInterest = ParseMoney(intForm.TAXEXEMPTINT),
                            });
                            break;
                        case Tax1099DIV_V100 divForm:
                            ret.Add(new Form1099Div()
                            {
                                PayerNameAndAddress = divForm.PAYERADDR.PAYERNAME1,
                                TotalOrdinaryDividends = ParseMoney(divForm.ORDDIV),
                                QualifiedDividends = ParseMoney(divForm.QUALIFIEDDIV),
                                NondividendDistributions = ParseMoney(divForm.NONTAXDIST),
                                FederalIncomeTaxWithheld = ParseMoney(divForm.FEDTAXWH),
                                Section199ADividends = ParseMoney(divForm.SEC199A),
                                ForeignTaxPaid = ParseMoney(divForm.FORTAXPD),
                                ExemptInterestDividends = ParseMoney(divForm.EXEMPTINTDIV),
                            });
                            break;
                        case Tax1099B_V100 stockForm:
                            foreach (var t in stockForm.EXTDBINFO_V100.PROCDET_V100)
                            {
                                DateTime? acquired = t.Item switch
                                {
                                    string s => ParseDate(s),
                                    BooleanType.Y => null,
                                    _ => throw new Exception($"Unsupported value for acquire time: {t.Item}"),
                                };
                                var code = Enum.Parse<Form8949Code>(t.FORM8949CODE);
                                transactions[code].Add(new Form8949Line(t.SALEDESCRIPTION, acquired, ParseDate(t.DTSALE), ParseMoney(t.COSTBASIS), ParseMoney(t.SALESPR), ParseMoney(t.TOTALADJ)));
                            }
                            break;
                        default:
                            throw new Exception($"Unsupported form type: {f.GetType().Name}");
                    }
                }
            }

            foreach (var kvp in transactions)
            {
                if (kvp.Value.Count != 0)
                {
                    ret.Add(new Form8949(kvp.Key, kvp.Value));
                }
            }

            return ret;
        }

        static decimal ParseMoney(string value)
        {
            if (value is null)
                return 0m;
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        static DateTime ParseDate(string value)
        {
            const string FORMAT_WITH_DATETIME = "yyyyMMddHHmmss";
            const string FORMAT_WITH_DATE = "yyyyMMdd";
            if (value.Length == FORMAT_WITH_DATE.Length)
                return DateTime.ParseExact(value, FORMAT_WITH_DATE, CultureInfo.InvariantCulture);
            else if (value.Length == FORMAT_WITH_DATETIME.Length)
                return DateTime.ParseExact(value, FORMAT_WITH_DATETIME, CultureInfo.InvariantCulture);
            else
                throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown date format.");
        }
    }
}
