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
#pragma warning disable 1591, 3021
namespace NClassify.Library
{
    public struct ValidationError
    {
        private static readonly ValidationError[] Empty = new ValidationError[0];
        private readonly global::System.Enum _field;
        private readonly string _message;
        private readonly ValidationError[] _errors;

        public ValidationError(global::System.Enum field, string message, params object[] args)
        {
            _field = field;
            _message = args.Length == 0 ? message : string.Format(message, args);
            _errors = Empty;
        }
        public ValidationError(global::System.Enum field, global::System.Collections.Generic.IEnumerable<ValidationError> errors)
        {
            _field = field;
            _message = null;
            _errors = new global::System.Collections.Generic.List<ValidationError>(errors).ToArray();
        }

        public static global::System.Collections.Generic.IEnumerable<ValidationError> EmptyList { get { return Empty; } }
        public global::System.Collections.Generic.IEnumerable<ValidationError> Errors { get { return _errors ?? Empty; } }

        [global::System.CLSCompliant(false)]
        public uint FieldId { get { return global::System.Convert.ToUInt32(_field); } }
        public string FieldName { get { return global::System.Convert.ToString(_field); } }
        public string Message { get { return _message ?? (_errors == null || _errors.Length == 0 ? string.Empty : string.Format(ResourceMessages.InvalidField, FieldName)); } }
        public bool HasError { get { return _message != null || _errors != null; } }

        public void RaiseException()
        {
            global::System.IO.InvalidDataException error = new global::System.IO.InvalidDataException(Message);
            error.Data["Validation"] = this;
            throw error;
        }
    }

    public interface IValidate
    {
        bool IsValid();
        void AssertValid();
        int GetBrokenRules(global::System.Action<ValidationError> onError);
    }

    public interface IBuilder : IMessage, global::System.Xml.Serialization.IXmlSerializable
    {
        bool IsReadOnly();
        void MakeReadOnly();

        void Clear();
        void AcceptDefaults();

        void MergeFrom(IMessage other);

        void ReadXml(string localName, global::System.Xml.XmlReader reader);
        void MergeFrom(global::System.Xml.XmlReader reader);
    }

    public interface IMessage : global::System.ICloneable, IValidate
    {
        void WriteXml(string localName, global::System.Xml.XmlWriter writer);
        void MergeTo(global::System.Xml.XmlWriter reader);
    }

    public interface IDispatchStub : global::System.IDisposable
    {
        void CallMethod<TRequest, TResponse>(string methodName, TRequest request, TResponse response)
            where TRequest : class, global::NClassify.Library.IMessage
            where TResponse : class, global::NClassify.Library.IBuilder;
    }

    public interface IServerStub
    {
        IMessage CallMethod(string methodName, global::System.Action<IBuilder> readInput);
    }
}
