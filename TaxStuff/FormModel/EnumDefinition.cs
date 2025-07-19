using System.Xml.Linq;

namespace TaxStuff.FormModel;

class EnumDefinition(XElement node) : CompoundDefinition<EnumElement>(node, "Value", el => new EnumElement(el))
{
}
