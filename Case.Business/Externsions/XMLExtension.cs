using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Case.Business
{
    public static class XMLExtension
    {
        public static string ToXmlString<T>(this T value, bool removeDefaultXmlNamespaces = true, bool omitXmlDeclaration = true, Encoding encoding = null) where T : class
        {
            XmlSerializerNamespaces namespaces = removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null;

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.CheckCharacters = false;

            using (var stream = new StringWriterWithEncoding(encoding))
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, namespaces);
                return stream.ToString();
            }
        }
    }

    public class StringWriterWithEncoding : StringWriter
    {
        public override Encoding Encoding => _encoding ?? base.Encoding;
        private readonly Encoding _encoding;
        public StringWriterWithEncoding() { }
        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }
    }
}
