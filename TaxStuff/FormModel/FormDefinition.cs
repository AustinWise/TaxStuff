using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using TaxStuff.ExpressionEvaluation;
using TaxStuff.ExpressionParsing;

namespace TaxStuff.FormModel;

class FormDefinition : IHasName
{
    public FormDefinition(string name, XDocument doc)
    {
        this.Name = name;

        if (doc.Root is null)
            throw new FileLoadException(doc, "Missing root element");

        this.AllowMultiple = doc.Root.OptionalBoolAttributeValue("AllowMultiple") ?? false;
        this.Calculateable = doc.Root.OptionalBoolAttributeValue("Calculateable") ?? true;
        this.RequireSsn = doc.Root.OptionalBoolAttributeValue("RequireSsn") ?? false;

        var enums = new Dictionary<string, EnumDefinition>();
        var structs = new Dictionary<string, StructDefinition>();
        var lines = new Dictionary<string, LineDefinition>();
        var lineByNumber = new Dictionary<string, LineDefinition>();
        var asserts = new List<AssertDefinition>();
        var env = new ParsingEnvironment()
        {
            CurrentFormName = name,
        };
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
                    foreach (var attr in node.Attributes())
                    {
                        switch (attr.Name.LocalName)
                        {
                            case "Number":
                            case "Name":
                            case "Calc":
                            case "Type":
                            case "Desc":
                                break;
                            default:
                                throw new FileLoadException(attr, "Unexpected attribute: " + attr.Name.LocalName);
                        }
                    }

                    var lineDef = lines.CheckNameAndAdd(node, new LineDefinition(env, node));
                    if (lineDef.Number is not null && !lineByNumber.TryAdd(lineDef.Number, lineDef))
                        throw new FileLoadException(node, $"Duplicate line number '{lineDef.Number}'.");
                    break;
                case "Assert":
                    // We don't support loading XML expressions on assert lines
                    if (node.HasElements)
                    {
                        throw new FileLoadException(node, "Assert lines should not have child elements.");
                    }

                    XAttribute? assertAttribute = null;
                    bool expectedValue = false;
                    foreach (var attr in node.Attributes())
                    {
                        switch (attr.Name.LocalName)
                        {
                            case "IsTrue":
                                if (assertAttribute is not null)
                                    throw new FileLoadException(attr, "Assert line has multiple IsTrue or IsFalse attributes.");
                                assertAttribute = attr;
                                expectedValue = true;
                                break;
                            case "IsFalse":
                                if (assertAttribute is not null)
                                    throw new FileLoadException(attr, "Assert line has multiple IsTrue or IsFalse attributes.");
                                assertAttribute = attr;
                                expectedValue = false;
                                break;
                            default:
                                throw new FileLoadException(attr, "Unexpected attribute: " + attr.Name.LocalName);
                        }
                    }

                    if (assertAttribute is null)
                        throw new FileLoadException(node, "Assert lines must have one IsTrue or one IsFalse attribute.");

                    BaseExpression assertExpr;
                    try
                    {
                        assertExpr = MyExpressionParser.Parse(env, assertAttribute.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new FileLoadException(assertAttribute, $"Failed to parse {assertAttribute.Name} expression.", ex);
                    }

                    asserts.Add(new AssertDefinition(assertAttribute.Value, assertExpr, expectedValue));
                    break;
                default:
                    throw new FileLoadException(node, $"Unexpected element '{node.Name}'.");
            }
        }


        this.Enums = new ReadOnlyDictionary<string, EnumDefinition>(enums);
        this.Structs = new ReadOnlyDictionary<string, StructDefinition>(structs);
        this.Lines = new ReadOnlyDictionary<string, LineDefinition>(lines);
        this.LinesByNumber = new ReadOnlyDictionary<string, LineDefinition>(lineByNumber);
        this.Asserts = new ReadOnlyCollection<AssertDefinition>(asserts);
    }

    public string Name { get; }

    /// <summary>
    /// Indicates that multiple forms of this type can be included in a return.
    /// For example multiple W-2 and 1099s can be include. 1040s can not.
    /// </summary>
    public bool AllowMultiple { get; }

    /// <summary>
    /// Indicates whether there are any values to calculate on this form.
    /// </summary>
    public bool Calculateable { get; }

    /// <summary>
    /// Indicates that the form instance must have a SSN attached to it.
    /// </summary>
    /// <remarks>
    /// For example, W-2s and 1040 Schedule SE is specific to one person
    /// and have single social security numbers on them.
    /// </remarks>
    public bool RequireSsn { get; }

    public ReadOnlyDictionary<string, EnumDefinition> Enums { get; }
    public ReadOnlyDictionary<string, StructDefinition> Structs { get; }
    public ReadOnlyDictionary<string, LineDefinition> Lines { get; }
    public ReadOnlyDictionary<string, LineDefinition> LinesByNumber { get; }
    public ReadOnlyCollection<AssertDefinition> Asserts { get; }
}
