
# TaxStuff - basic federal tax return calculator

NOTE: this is a work in progress; please don't file your taxes based solely on this software. I don't.

This software attempts to automate a subset of the United States Internal Revenue Service forms. The
intention is to enable the user of the software to estimate their tax liability and to better
understand their taxes. The software is not a substitute for something like TurboTax: it assumes you
already know what forms you want to file.

I was inspired by Robert Sesek's excellent [ustaxlib](https://github.com/rsesek/ustaxlib).

# Example

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) to build
and run.

When you run the program, it takes two arguments: the path to the return file
and a folder to put the PDFs in:

```bash
mkdir output
dotnet run --project TaxStuff/TaxStuff.csproj ExampleReturn.xml output
```

See example [input file](ExampleReturn.xml) and example
[1040 output](Example1040.pdf).

# Features

* Can import OFX tax exports (from Schwab for example), to support interest
  dividends, and stock transactions. The contents of the 1099-Bs will be turned
  into Form 8949s and will be referenced by !040 Schedule D. The 1009-DIVs and
  1099-INTs will be put into the right places on Form 1040.
* Fill in the results of tax computations into PDF tax forms. (only implemented for the 2020 tax year currently)
* A custom XML format and expression language for defining tax forms. See the
  [2020 folder](./TaxStuff/2020/) for the forms.

# License

I'm not really sure what to license this as right now. The dependencies has a
couple of different license, which makes this more complicated.

* iText: GNU Affero General Public License
* ANTLR: BSD 3-clause
* [OFX](https://www.ofx.net/) XML Schema files: Just "All Rights Reserved",
  though the specification says
  > A royalty-free, worldwide, and perpetual
  > license is hereby granted to any party to use the Open Financial Exchange
  > Specification to make, use, and sell products and services that conform to
  > this Specification.

  I don't know how "conform" is evaluated.

# TODO

* Add an `Assert` element in forms to check for errors.
* More unit tests
* Support for references to previous years, to support things like capital loss
  carryover and Schedule J.
* Support for filling in personal information into PDFs.
* Support for filtering array values, probably using a bracket syntax. Use cases:
  * Replace the special filter function `FilterForm8949` with native filitering
     support.
* Similar to the above filtering, support a "group by" feature.
  * For 2020 Form 1040 Schedule 3 Line 10, it would be nice  to write something
      like:

      ```c#
      FormW-2.GroupBy(f => f.SSN)
             .Select(g => Math.Max(0, g.Sum(f => f.SocialSecurityTaxWithheld) - 8537.40))
             .Sum()
      ```

* Clean up PDF writing
  * Leave spaces blank when appropriate instead of writing 0.
  * Round to whole dollars.
* Somehow unify the parsing, typechecking, and evaluation representation of
  language semantics. Particularly `EvaluationResult` and `ExpressionType` have
  a similar shape.
* Maybe instead of interpreting the expression language, it could be compiled
  to C# using a source generator. This might allow for the execution engine
  having type-safe knowledge of different forms, for the benefit of the OXF
  transaction importer.

# Adding support for filling in PDFs

I've only done 1040, the process I'm using to find the name of the fields of the
PDF to fill in is:

1. Use the `XfaForm` iText API to pull out the [XFA form](https://en.wikipedia.org/wiki/XFA)
   template.
1. Find the assistive text associated with each field. These nodes can be found
   with the XPath expression `form/assist/speak`.
1. Record a substring of this node in the PdfInfo.xml file.

Hopefully by going by the assistive text on the PDF this mapping is a little
easier to maintain than just recording the field name. The field names appear
to be generated, like "f2_28".
