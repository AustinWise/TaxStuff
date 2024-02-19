using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TaxStuff.FormModel;

namespace TaxStuff.ExpressionEvaluation;

class TypecheckEnvironment
{
    readonly Stack<(FormDefinition, LineDefinition)> mRecursionTracker = new();

    /// <summary>
    /// For use when type checking lines.
    /// </summary>
    public TypecheckEnvironment(ReadOnlyDictionary<string, FormDefinition> forms, FormDefinition currentForm, LineDefinition currentLine)
    {
        this.Forms = forms;
        this.CurrentForm = currentForm;
        mRecursionTracker.Push((currentForm, currentLine));
    }

    /// <summary>
    /// For use when type checking asserts.
    /// </summary>
    public TypecheckEnvironment(ReadOnlyDictionary<string, FormDefinition> forms, FormDefinition currentForm)
    {
        this.Forms = forms;
        this.CurrentForm = currentForm;
    }

    /// <summary>
    /// For use when parsing the Return. There should be all literal values, not form references.
    /// </summary>
    public TypecheckEnvironment()
    {
        this.Forms = new ReadOnlyDictionary<string, FormDefinition>(new Dictionary<string, FormDefinition>());
    }

    public ReadOnlyDictionary<string, FormDefinition> Forms { get; }
    public FormDefinition? CurrentForm { get; private set; }

    public void PushRecursionCheck(FormDefinition form, LineDefinition line)
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentNullException.ThrowIfNull(line);

        bool foundRecursion = false;
        foreach (var tup in mRecursionTracker)
        {
            if (ReferenceEquals(tup.Item1, form) && ReferenceEquals(tup.Item2, line))
            {
                foundRecursion = true;
                break;
            }
        }

        if (foundRecursion)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Recursion detected in expression definition:");
            foreach (var tup in mRecursionTracker.Reverse().SkipWhile(t => !(ReferenceEquals(t.Item1, form) && ReferenceEquals(t.Item2, line))))
            {
                sb.AppendLine($"Form{tup.Item1.Name}.Line{tup.Item2.Number}");
            }
            sb.AppendLine($"Form{form.Name}.Line{line.Number}");
            string message = sb.ToString();
            throw new Exception(message);
        }

        mRecursionTracker.Push((form, line));
        CurrentForm = form;
    }

    public void PopRecursionCheck()
    {
        mRecursionTracker.Pop();
        CurrentForm = mRecursionTracker.Count == 0 ? null : mRecursionTracker.Peek().Item1;
    }
}
