using System.Xml.Linq;

namespace TaxStuff.FormModel
{
    class StructDefinition : CompoundDefinition<StructElement>
    {
        public StructDefinition(XElement node)
            : base(node, "Field", el => new StructElement(el))
        {
        }
    }
}
