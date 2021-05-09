using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace TaxTest.FormModel
{
    abstract class CompoundDefinition<TElementType> : IHasName where TElementType : IHasName
    {
        public string Name { get; }
        public ReadOnlyCollection<TElementType> Elements { get; }

        protected CompoundDefinition(XElement node, string elementName, Func<XElement, TElementType> factory)
        {
            this.Name = node.AttributeValue("Name");

            var elements = new List<TElementType>();
            var names = new HashSet<string>();
            foreach (var el in node.Elements())
            {
                if (el.Name == elementName)
                {
                    var element = factory(el);
                    if (!names.Add(element.Name))
                        throw new FileLoadException(el, $"Duplicate element named '{element.Name}'.");
                    elements.Add(element);
                }
                else
                {
                    throw new FileLoadException(el, $"Unexpected element '{el.Name}'.");
                }
            }
            this.Elements = new ReadOnlyCollection<TElementType>(elements);
        }
    }
}
