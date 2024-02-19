using System.Xml.Linq;

namespace TaxStuff.FormModel;

record EnumElement(string Name, string Description) : IHasName
{
    public EnumElement(XElement el)
        : this(el.AttributeValue("Name"), el.AttributeValue("Description"))
    {
    }
}
