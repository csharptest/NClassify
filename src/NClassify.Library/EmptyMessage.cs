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
#pragma warning disable 1591
namespace NClassify.Library
{
    public sealed class EmptyMessage : IBuilder
    {
        private static readonly EmptyMessage Instance = new EmptyMessage();
        public static EmptyMessage DefaultInstance { get { return Instance; } }

        bool IBuilder.IsReadOnly() { return false; }
        void IBuilder.MakeReadOnly() { }
        void IBuilder.Clear() { }
        void IBuilder.AcceptDefaults() { }
        void IBuilder.MergeFrom(IMessage reader) { }
        void IBuilder.ReadXml(string localName, global::System.Xml.XmlReader reader) { }
        void IBuilder.MergeFrom(global::System.Xml.XmlReader reader) { }
        void IMessage.WriteXml(string localName, global::System.Xml.XmlWriter writer) { }
        void IMessage.MergeTo(global::System.Xml.XmlWriter reader) { }
        object global::System.ICloneable.Clone() { return this; }
        bool IValidate.IsValid() { return true; }
        void IValidate.AssertValid() { }
        int IValidate.GetBrokenRules(global::System.Action<ValidationError> onError) { return 0; }
        global::System.Xml.Schema.XmlSchema global::System.Xml.Serialization.IXmlSerializable.GetSchema() { return null; }
        void global::System.Xml.Serialization.IXmlSerializable.ReadXml(global::System.Xml.XmlReader reader) { }
        void global::System.Xml.Serialization.IXmlSerializable.WriteXml(global::System.Xml.XmlWriter writer) { }
    }
}