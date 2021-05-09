using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace TaxTest.FormModel
{
    record TaxComputationWorksheetSection(FilingStatus Status, ReadOnlyCollection<TaxComputationWorksheetLine> Lines)
    {
        public TaxComputationWorksheetSection(XElement node)
            : this(node.EnumAttributeValue<FilingStatus>("Status"),
                   new ReadOnlyCollection<TaxComputationWorksheetLine>(node.Elements("Case").Select(n => new TaxComputationWorksheetLine(n)).OrderBy(n => n.Min).ToList()))
        {
        }
    }
}
