using System.Xml.Linq;

namespace TaxTest.FormModel
{
    record StructElement(string Name, string Type) : IHasName
    {
        public StructElement(XElement el)
            : this(el.AttributeValue("Name"), el.AttributeValue("Type"))
        {
        }
    }
}
