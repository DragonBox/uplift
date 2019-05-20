// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Uplift.Common
{
	public class StrictXmlDeserializer<T>
	{
		XmlSerializer serializer;

		// deserialization state, not thread friendly
		List<String> errors;

		public StrictXmlDeserializer()
		{
			this.serializer = new XmlSerializer(typeof(T));
		}

		public T Deserialize(Stream stream)
		{
			return StrictDeserialize(
				() => { return serializer.Deserialize(stream); }
			);
		}

		public T Deserialize(string xmlString)
		{
			return Deserialize(new MemoryStream(Convert.FromBase64String(xmlString)));
		}

		private T StrictDeserialize(Func<Object> block)
		{
			errors = new List<string>();

			var Node = new XmlNodeEventHandler(serializer_UnknownNode);
			var Attr = new XmlAttributeEventHandler(serializer_UnknownAttribute);
			var Element = new XmlElementEventHandler(serializer_UnknownElement);

			serializer.UnknownNode += Node;
			serializer.UnknownAttribute += Attr;
			serializer.UnknownElement += Element;

			T result;
			try
			{
				result = (T)block();
			}
			finally
			{
				serializer.UnknownNode -= Node;
				serializer.UnknownAttribute -= Attr;
				serializer.UnknownElement -= Element;
			}

			if (errors.Count > 0)
			{
				throw new System.Exception("XML for " + typeof(T) + " has invalid format:\n" + string.Join("\n", errors.ToArray()));
			}

			return result;
		}

		private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(String.Format("UnknownNode Name: {0}", e.Name));
			sb.Append(String.Format("UnknownNode LocalName: {0}", e.LocalName));
			sb.Append(String.Format("UnknownNode Namespace URI: {0}", e.NamespaceURI));
			sb.Append(String.Format("UnknownNode Text: {0}", e.Text));

			XmlNodeType myNodeType = e.NodeType;
			sb.Append(String.Format("NodeType: {0}", myNodeType));
			errors.Add(sb.ToString());
			/*
			T x  = (T) e.ObjectBeingDeserialized;
			*/
		}

		private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Unknown Attribute");
			sb.Append("\t" + e.Attr.Name + " " + e.Attr.InnerXml);
			sb.Append("\t LineNumber: " + e.LineNumber);
			sb.Append("\t LinePosition: " + e.LinePosition);
			errors.Add(sb.ToString());
			/*
			T x  = (T) e.ObjectBeingDeserialized;
			*/
		}
		private void serializer_UnknownElement(object sender, XmlElementEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Unknown Element");
			sb.Append("\t" + e.Element.Name + " " + e.Element.InnerXml);
			sb.Append("\t LineNumber: " + e.LineNumber);
			sb.Append("\t LinePosition: " + e.LinePosition);
			errors.Add(sb.ToString());
			/*
			T x  = (T) e.ObjectBeingDeserialized;
			*/
		}
	}
}