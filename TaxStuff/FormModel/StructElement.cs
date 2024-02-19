using System.Xml.Linq;

namespace TaxStuff.FormModel;

record StructElement(string Name, string Type) : IHasName
{
    public StructElement(XElement el)
        : this(el.AttributeValue("Name"), el.AttributeValue("Type"))
    {
    }
}
