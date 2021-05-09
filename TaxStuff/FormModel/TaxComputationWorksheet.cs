using System.Collections.Generic;
using System.Xml.Linq;

namespace TaxStuff.FormModel
{
    class TaxComputationWorksheet
    {
        readonly Dictionary<FilingStatus, TaxComputationWorksheetSection> mSections;

        public TaxComputationWorksheet(XDocument doc)
        {
            mSections = new();
            foreach (var el in doc.Root.Elements("Section"))
            {
                var section = new TaxComputationWorksheetSection(el);
                mSections.Add(section.Status, section);
            }
        }

        public TaxComputationWorksheetSection GetSection(FilingStatus status) => mSections[status];
    }
}
