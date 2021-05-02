using System;
using System.Xml;

namespace TaxTest.FormModel
{
    class FileLoadException : Exception
    {
        static string getMessage(IXmlLineInfo element, string message)
        {
            if (element.HasLineInfo())
            {
                return $"Line {element.LineNumber}: {message}";
            }
            else
            {
                return message;
            }
        }

        public FileLoadException(IXmlLineInfo element, string message)
            : base(getMessage(element, message))
        {
        }

        public FileLoadException(IXmlLineInfo element, string message, Exception innerException)
            : base(getMessage(element, message), innerException)
        {
        }

        public FileLoadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
