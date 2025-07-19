using System.Xml.Linq;

namespace TaxStuff.FormModel;

class StructDefinition(XElement node) : CompoundDefinition<StructElement>(node, "Field", el => new StructElement(el))
{
}
