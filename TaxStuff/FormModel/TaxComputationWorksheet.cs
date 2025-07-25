﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TaxStuff.FormModel;

class TaxComputationWorksheet
{
    readonly Dictionary<FilingStatus, TaxComputationWorksheetSection> mSections;

    public TaxComputationWorksheet(XDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc.Root);

        mSections = [];
        foreach (var el in doc.Root.Elements("Section"))
        {
            var section = new TaxComputationWorksheetSection(el);
            mSections.Add(section.Status, section);
        }
    }

    public TaxComputationWorksheetSection GetSection(FilingStatus status) => mSections[status];
}
