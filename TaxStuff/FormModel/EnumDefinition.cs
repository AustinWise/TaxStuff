using System.Xml.Linq;

namespace TaxStuff.FormModel;

class EnumDefinition : CompoundDefinition<EnumElement>
{
    public EnumDefinition(XElement node)
        : base(node, "Value", el => new EnumElement(el))
    {
    }
}
