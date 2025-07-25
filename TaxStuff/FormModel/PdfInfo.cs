﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TaxStuff.FormModel;

class PdfInfo
{
    public PdfInfo(string folderPath, XDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc.Root);

        Forms = [];

        foreach (var node in doc.Root.Elements("Form"))
        {
            var formInfo = new FormPdfInfo(folderPath, node);
            Forms.Add(formInfo.FormName, formInfo);
        }
    }

    public Dictionary<string, FormPdfInfo> Forms { get; }
}
