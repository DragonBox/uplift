using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Uplift.Common
{
    public class StrictXmlDeserializer<T>
    {
        XmlSerializer serializer;

        // deserialization state, not thread friendly
        List<String> errors;

        public StrictXmlDeserializer() {
            this.serializer = new XmlSerializer(typeof(T));
        }

        public T Deserialize(FileStream fileStream) {
            return StrictDeserialize(
                () => { return serializer.Deserialize(fileStream); }
            );
        }

        private T StrictDeserialize(Func<Object> block) {
            errors = new List<string> ();

            var Node = new XmlNodeEventHandler(serializer_UnknownNode);
            var Attr = new XmlAttributeEventHandler (serializer_UnknownAttribute);
            var Element = new XmlElementEventHandler (serializer_UnknownElement);

            serializer.UnknownNode += Node;
            serializer.UnknownAttribute += Attr;
            serializer.UnknownElement += Element;

            T result;
            try {
                result = (T) block();
            } finally {
                serializer.UnknownNode -= Node;
                serializer.UnknownAttribute -= Attr;
                serializer.UnknownElement -= Element;
            }

            if (errors.Count > 0) {
                throw new System.Exception("XML for " + typeof(T) + " has invalid format:\n" + string.Join("\n", errors.ToArray()));
            }

            return result;
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e) {
            StringBuilder sb = new StringBuilder ();
            sb.Append(String.Format("UnknownNode Name: {0}", e.Name));
            sb.Append(String.Format("UnknownNode LocalName: {0}" ,e.LocalName));
            sb.Append(String.Format("UnknownNode Namespace URI: {0}", e.NamespaceURI));
            sb.Append(String.Format("UnknownNode Text: {0}", e.Text));

            XmlNodeType myNodeType = e.NodeType;
            sb.Append(String.Format("NodeType: {0}", myNodeType));
            errors.Add (sb.ToString ());
            /*
            T x  = (T) e.ObjectBeingDeserialized;
            */
        }

       private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e) {
            StringBuilder sb = new StringBuilder ();
            sb.Append("Unknown Attribute");
            sb.Append("\t" + e.Attr.Name + " " + e.Attr.InnerXml);
            sb.Append("\t LineNumber: " + e.LineNumber);
            sb.Append("\t LinePosition: " + e.LinePosition);
            errors.Add (sb.ToString ());
            /*
            T x  = (T) e.ObjectBeingDeserialized;
            */
        }
        private void serializer_UnknownElement(object sender, XmlElementEventArgs e) {
            StringBuilder sb = new StringBuilder ();
            sb.Append("Unknown Element");
            sb.Append("\t" + e.Element.Name + " " + e.Element.InnerXml);
            sb.Append("\t LineNumber: " + e.LineNumber);
            sb.Append("\t LinePosition: " + e.LinePosition);
            errors.Add (sb.ToString ());
            /*
            T x  = (T) e.ObjectBeingDeserialized;
            */
        }
    }
}
