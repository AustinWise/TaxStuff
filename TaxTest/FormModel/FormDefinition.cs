using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TaxTest.FormModel
{
    class FormDefinition : IHasName
    {
        public static FormDefinition LoadFromFile(string filePath)
        {
            string name = Path.GetFileNameWithoutExtension(filePath);
            using var reader = new StreamReader(filePath);
            try
            {
                return new FormDefinition(name, reader);
            }
            catch (FileLoadException ex)
            {
                throw new FileLoadException($"Error loading '{filePath}'.", ex);
            }
        }

        private FormDefinition(string name, TextReader reader)
        {
            this.Name = name;

            var doc = XDocument.Load(reader, LoadOptions.SetLineInfo);

            this.AllowMultiple = doc.Root.OptionalBoolAttributeValue("AllowMultiple") ?? false;
            this.Calculateable = doc.Root.OptionalBoolAttributeValue("Calculateable") ?? true;

            var enums = new Dictionary<string, EnumDefinition>();
            var structs = new Dictionary<string, StructDefinition>();
            var lines = new Dictionary<string, LineDefinition>();
            var lineByNumber = new Dictionary<string, LineDefinition>();
            foreach (var node in doc.Root.Elements())
            {
                switch (node.Name.LocalName)
                {
                    case "Enum":
                        enums.CheckNameAndAdd(node, new EnumDefinition(node));
                        break;
                    case "Struct":
                        structs.CheckNameAndAdd(node, new StructDefinition(node));
                        break;
                    case "Line":
                        var lineDef = lines.CheckNameAndAdd(node, new LineDefinition(node));
                        if (!lineByNumber.TryAdd(lineDef.Number, lineDef))
                            throw new FileLoadException(node, $"Duplicate line number '{lineDef.Number}'.");
                        break;
                    default:
                        throw new FileLoadException(node, $"Unexpected element '{node.Name}'.");
                }
            }


            this.Enums = new ReadOnlyDictionary<string, EnumDefinition>(enums);
            this.Structs = new ReadOnlyDictionary<string, StructDefinition>(structs);
            this.Lines = new ReadOnlyDictionary<string, LineDefinition>(lines);
            this.LinesByNumber = new ReadOnlyDictionary<string, LineDefinition>(lineByNumber);
        }

        public string Name { get; }

        public bool AllowMultiple { get; }
        public bool Calculateable { get; }

        public ReadOnlyDictionary<string, EnumDefinition> Enums { get; }
        public ReadOnlyDictionary<string, StructDefinition> Structs { get; }
        public ReadOnlyDictionary<string, LineDefinition> Lines { get; }
        public ReadOnlyDictionary<string, LineDefinition> LinesByNumber { get; }
    }
}
