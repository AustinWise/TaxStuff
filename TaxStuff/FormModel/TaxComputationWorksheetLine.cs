﻿using System.Xml.Linq;

namespace TaxStuff.FormModel;

record TaxComputationWorksheetLine(decimal Min, decimal MultiplicationAmount, decimal SubtractionAmount)
{
    public TaxComputationWorksheetLine(XElement node)
        : this(node.DecimalAttributeValue("Min"),
               node.DecimalAttributeValue("MultiplicationAmount"),
               node.DecimalAttributeValue("SubtractionAmount"))
    {
    }
}
