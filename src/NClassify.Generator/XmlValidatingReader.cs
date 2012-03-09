#region Copyright (c) 2012 Roger O Knapp
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NClassify.Generator
{
    /// <summary>
    /// Validates and loads an xml file based on an embeded xml schema
    /// </summary>
    /// <typeparam name="T">The type used for xml serialization</typeparam>
    class XmlValidatingReader<T>
    {
        private readonly XmlSchema _xmlSchema;
        
        /// <summary>
        /// Provide the full manifest resource name of the Xml Schema file to load
        /// </summary>
		public XmlValidatingReader(string resourceName)
        {
            if (resourceName == null)
                throw new ArgumentNullException("resourceName");

            using (Stream xsd = typeof(T).Assembly.GetManifestResourceStream(resourceName))
            {
                if (xsd == null)
                    throw new ArgumentException("Resource " + resourceName + " not found.", "resourceName");
                _xmlSchema = XmlSchema.Read(XmlReader.Create(xsd), null);
            }
		}

        /// <summary>
        /// Deserialize the xml file directly from an XmlReader instance
        /// </summary>
        public T ReadXml(string inputUri)
        {
            using(XmlReader rdr = XmlReader.Create(inputUri))
                return ReadXml(rdr);
        }

		/// <summary>
		/// Deserialize the xml file directly from an XmlReader instance
		/// </summary>
		public T ReadXml(XmlReader reader)
		{
		    T data = default(T);
            ValidatedRead(reader, r => data = (T)new XmlSerializer(typeof(T)).Deserialize(r));
            return data;
		}

        /// <summary>
        /// Operates on the reader using schema validation
        /// </summary>
        public void ValidatedRead(XmlReader reader, Action<XmlReader> read)
        {
            List<XmlException> parseErrors = new List<XmlException>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = false;
            settings.CheckCharacters = true;
            settings.Schemas.Add(_xmlSchema);
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler +=
                delegate(object sender, ValidationEventArgs e)
                    {
                        parseErrors.Add(
                            new XmlException(e.Message, e.Exception, e.Exception.LineNumber, e.Exception.LinePosition)
                                {Source = e.Exception.SourceUri ?? reader.BaseURI}
                            );
                    };

            try
            {
                using (XmlReader validation = XmlReader.Create(reader, settings))
                    read(validation);
            }
            catch (Exception e)
            {
                while (e != null)
                {
                    XmlException xe = e as XmlException;
                    if (xe != null)
                    {
                        xe.Source = xe.SourceUri ?? reader.BaseURI;
                        parseErrors.Add(xe);
                        break;
                    }
                    e = e.InnerException;
                }
                if (parseErrors.Count == 0)
                    throw;
            }


            if (parseErrors.Count > 0)
            {
                StringBuilder msg = new StringBuilder();
                foreach (XmlException e in parseErrors)
                {
                    string src = String.Empty;
                    Uri tmp;
                    if (Uri.TryCreate(e.Source, UriKind.Absolute, out tmp))
                        src = tmp.AbsolutePath;
                    msg.AppendFormat("{0}({1},{2}): {3}{4}", src,
                                     e.LineNumber, e.LinePosition,
                                     e.Message,
                                     Environment.NewLine);
                }

                msg.AppendFormat("{0} error(s) found starting at", parseErrors.Count);
                System.Diagnostics.Debug.WriteLine(msg);
                throw new XmlException(msg.ToString(), parseErrors[0],
                                       parseErrors[0].LineNumber,
                                       parseErrors[0].LinePosition)
                          {
                              Source = parseErrors[0].Source
                          };
            }
        }
    }
}
