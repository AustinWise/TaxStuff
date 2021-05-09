
# TaxStuff - basic federal tax return calculator

NOTE: work in progress, not ready to be used by anyone for anything other than
their own amusement.

How hard could it be to clone TruboTax. Quite hard, given the complexity of the
tax code. So this is not really user-friendly. It is about on the level of
[Free File Fillable Forms](https://www.irs.gov/e-file-providers/free-file-fillable-forms):
it assumes you understand the tax code enough to know which forms you need to
file.

I was inspired by Robert Sesek's excellent [ustaxlib](https://github.com/rsesek/ustaxlib).

Unlike ustaxlib and Open Tax Solver, this program defines its own file format
and expression language for describing tax forms. The goal was constrain the
the system from using arbitrary computation, to hopefully make the form
definitions easier to comprehend and easier to implement correctly. At the same
by providing a language and format tailored for tax forms, the forms can be more
easily translated. I'm not sure this program really succeeds at either goal, as
I have not implemented that many forms

This program has the ability to file in PDFs with the result of computations.

# Example

See example [input file](ExampleReturn.xml) and example
[1040 output](Example1040.pdf).

When you run the program, it takes two arguments: the path to the return file
and a folder to put the PDFs in:

```bash
mkdir output
dotnet run --project TaxTest/TaxTest.csproj ExampleReturn.xml output
```

# License

I'm not really sure what to license this as right now. The dependencies has a
couple of different license, which makes this more complicated.

* iText: GNU Affero General Public License
* ANTLR: BSD 3-clause

# TODO

* Add an `Assert` element in forms to check for errors.
* Unit tests probably
* Support for parsing 1099-B forms and generated form 8949 and Schedule D automatically.
* Support for references to previous years, to support things like capital loss
  carryover and Schedule J.

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
