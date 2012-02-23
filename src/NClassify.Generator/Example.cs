// Generated by NClassify.Generator, Version=1.0
#pragma warning disable 1591, 0612, 3021
#region Designer generated code
namespace NClassify.Generator
{
    #region asdf
    [global::System.Xml.Serialization.XmlType("asdf")]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public enum Asdf
    {
        /// <summary>
        /// asdf = 1
        /// </summary>
        [global::System.Xml.Serialization.XmlEnum("asdf")] Asdf = 1,
        /// <summary>
        /// fdsa = 2
        /// </summary>
        [global::System.Xml.Serialization.XmlEnum("fdsa")] Fdsa = 2,
        /// <summary>
        /// sdfa = 3
        /// </summary>
        [global::System.Xml.Serialization.XmlEnum("sdfa")] Sdfa = 3,
    }
    #endregion
    #region callback_id
    [global::System.Xml.Serialization.XmlType("callback_id")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial struct CallbackId : global::System.IEquatable<CallbackId>, global::System.IComparable<CallbackId>
    {
        #region Instance Fields and Members
        public static bool IsValidValue(ulong value)
        {
            if (value.CompareTo(1ul) < 0) return false;
            return true;
        }
        private bool __has_value;
        private ulong __value;
        [global::System.CLSCompliant(false)]
        public CallbackId(ulong value) : this()
        {
            this.Value = value;
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasValue
        {
            get
            {
                return __has_value;
            }
        }
        [global::System.Xml.Serialization.XmlText()]
        public ulong Value
        {
            get
            {
                if (!__has_value) throw new global::System.InvalidOperationException();
                return __value;
            }
            private set
            {
                if (!IsValidValue(value)) throw new global::System.ArgumentOutOfRangeException("Value");
                __value = value;
                __has_value = true;
            }
        }
        #endregion
        #region Operators and Comparisons
        public override string ToString()
        {
            return __has_value ? __value.ToString() : null;
        }
        public override int GetHashCode()
        {
            return __has_value ? __value.GetHashCode() : 0;
        }
        public override bool Equals(object obj)
        {
            return obj is CallbackId ? Equals((CallbackId)obj) : base.Equals(obj);
        }
        public bool Equals(CallbackId other)
        {
            return __has_value && other.__has_value ? __value.Equals(other.__value) : __has_value == other.__has_value;
        }
        public int CompareTo(CallbackId other)
        {
            return __has_value && other.__has_value ? __value.CompareTo(other.__value) : __has_value ? 1 : __has_value ? -1 : 0;
        }
        [global::System.CLSCompliant(false)]
        public static explicit operator CallbackId(ulong value)
        {
            return new CallbackId(value);
        }
        [global::System.CLSCompliant(false)]
        public static explicit operator ulong(CallbackId value)
        {
            return value.Value;
        }
        public static bool operator ==(CallbackId x, CallbackId y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(CallbackId x, CallbackId y)
        {
            return !x.Equals(y);
        }
        #endregion
    }
    #endregion
    #region web_uri
    [global::System.Xml.Serialization.XmlType("web_uri")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial struct WebUri : global::System.IEquatable<WebUri>, global::System.IComparable<WebUri>
    {
        #region Instance Fields and Members
        private static readonly global::System.Text.RegularExpressions.Regex __valid_value = new global::System.Text.RegularExpressions.Regex("^(https?\\://.+", global::System.Text.RegularExpressions.RegexOptions.Singleline | global::System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        private static bool IsValidUri(string value)
        {
            global::System.Uri tmp;
            return global::System.Uri.TryCreate(value, global::System.UriKind.Absolute, out tmp);
        }
        public static bool IsValidValue(string value)
        {
            if (null == value) return false;
            if (value.Length < 8) return false;
            if (value.Length > 2048) return false;
            if (__valid_value.IsMatch(value.ToString())) return false;
            if (!(IsValidUri(value))) return false;
            return true;
        }
        private bool __has_value;
        private string __value;
        public WebUri(string value) : this()
        {
            this.Value = value;
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasValue
        {
            get
            {
                return __has_value;
            }
        }
        [global::System.Xml.Serialization.XmlText()]
        public string Value
        {
            get
            {
                if (!__has_value) throw new global::System.InvalidOperationException();
                return __value;
            }
            private set
            {
                if (!IsValidValue(value)) throw new global::System.ArgumentOutOfRangeException("Value");
                __value = value;
                __has_value = true;
            }
        }
        #endregion
        #region Operators and Comparisons
        public override string ToString()
        {
            return __has_value ? __value.ToString() : null;
        }
        public override int GetHashCode()
        {
            return __has_value ? __value.GetHashCode() : 0;
        }
        public override bool Equals(object obj)
        {
            return obj is WebUri ? Equals((WebUri)obj) : base.Equals(obj);
        }
        public bool Equals(WebUri other)
        {
            return __has_value && other.__has_value ? __value.Equals(other.__value) : __has_value == other.__has_value;
        }
        public int CompareTo(WebUri other)
        {
            return __has_value && other.__has_value ? __value.CompareTo(other.__value) : __has_value ? 1 : __has_value ? -1 : 0;
        }
        public static explicit operator WebUri(string value)
        {
            return new WebUri(value);
        }
        public static explicit operator string(WebUri value)
        {
            return value.Value;
        }
        public static bool operator ==(WebUri x, WebUri y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(WebUri x, WebUri y)
        {
            return !x.Equals(y);
        }
        #endregion
    }
    #endregion
    #region event_type
    [global::System.Xml.Serialization.XmlType("event_type")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial struct EventType : global::System.IEquatable<EventType>, global::System.IComparable<EventType>
    {
        #region Instance Fields and Members
        private static readonly string[] __in_value = new string[] {"all", "category", "category.create", "category.delete", "category.update", "client", "client.create", "client.delete", "client.update", "estimate", "estimate.create", "estimate.delete", "estimate.sendByEmail", "estimate.update", "expense", "expense.create", "expense.delete", "expense.update", "invoice", "invoice.create", "invoice.delete", "invoice.dispute", "invoice.pastdue.1", "invoice.pastdue.2", "invoice.pastdue.3", "invoice.sendByEmail", "invoice.sendBySnailMail", "invoice.update", "item", "item.create", "item.delete", "item.update", "payment", "payment.create", "payment.delete", "payment.update", "project", "project.create", "project.delete", "project.update", "recurring", "recurring.create", "recurring.delete", "recurring.update", "staff", "staff.create", "staff.delete", "staff.update", "task", "task.create", "task.delete", "task.update", "time_entry", "time_entry.create", "time_entry.delete", "time_entry.update"};
        public static bool IsValidValue(string value)
        {
            if (null == value) return false;
            if (global::System.Array.BinarySearch(__in_value, value) >= 0) return false;
            return true;
        }
        private bool __has_value;
        private string __value;
        public EventType(string value) : this()
        {
            this.Value = value;
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasValue
        {
            get
            {
                return __has_value;
            }
        }
        [global::System.Xml.Serialization.XmlText()]
        public string Value
        {
            get
            {
                if (!__has_value) throw new global::System.InvalidOperationException();
                return __value;
            }
            private set
            {
                if (!IsValidValue(value)) throw new global::System.ArgumentOutOfRangeException("Value");
                __value = value;
                __has_value = true;
            }
        }
        #endregion
        #region Operators and Comparisons
        public override string ToString()
        {
            return __has_value ? __value.ToString() : null;
        }
        public override int GetHashCode()
        {
            return __has_value ? __value.GetHashCode() : 0;
        }
        public override bool Equals(object obj)
        {
            return obj is EventType ? Equals((EventType)obj) : base.Equals(obj);
        }
        public bool Equals(EventType other)
        {
            return __has_value && other.__has_value ? __value.Equals(other.__value) : __has_value == other.__has_value;
        }
        public int CompareTo(EventType other)
        {
            return __has_value && other.__has_value ? __value.CompareTo(other.__value) : __has_value ? 1 : __has_value ? -1 : 0;
        }
        public static explicit operator EventType(string value)
        {
            return new EventType(value);
        }
        public static explicit operator string(EventType value)
        {
            return value.Value;
        }
        public static bool operator ==(EventType x, EventType y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(EventType x, EventType y)
        {
            return !x.Equals(y);
        }
        #endregion
    }
    #endregion
    #region Callback
    [global::System.Xml.Serialization.XmlType("Callback")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class Callback
    {
        #region Static Data
        public static bool IsValidCallbackId(global::NClassify.Generator.CallbackId testValue)
        {
            if (!testValue.HasValue) return false;
            ulong value = (ulong)testValue;
            if (!NClassify.Generator.CallbackId.IsValidValue(value)) return false;
            return true;
        }
        public static bool IsValidUri(global::NClassify.Generator.WebUri testValue)
        {
            if (!testValue.HasValue) return false;
            string value = (string)testValue;
            if (!NClassify.Generator.WebUri.IsValidValue(value)) return false;
            return true;
        }
        public static bool IsValidEvent(global::NClassify.Generator.EventType testValue)
        {
            if (!testValue.HasValue) return false;
            string value = (string)testValue;
            if (!NClassify.Generator.EventType.IsValidValue(value)) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_callbackId;
        private ulong __callbackId;
        private bool __has_uri;
        private string __uri;
        private bool __has_event;
        private string __event;
        private bool __has_verified;
        private bool __verified;
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasCallbackId
        {
            get
            {
                return __has_callbackId;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __callbackId = 0ul;
                __has_callbackId = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.CallbackId))]
        [global::System.Xml.Serialization.XmlElement("callback_id")]
        public global::NClassify.Generator.CallbackId CallbackId
        {
            get
            {
                return (global::NClassify.Generator.CallbackId)__callbackId;
            }
            set
            {
                if (!IsValidCallbackId(value)) throw new global::System.ArgumentOutOfRangeException("CallbackId");
                __callbackId = (ulong)value;
                __has_callbackId = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasUri
        {
            get
            {
                return __has_uri;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __uri = "";
                __has_uri = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.WebUri))]
        [global::System.Xml.Serialization.XmlElement("uri")]
        public global::NClassify.Generator.WebUri Uri
        {
            get
            {
                return (global::NClassify.Generator.WebUri)__uri;
            }
            set
            {
                if (!IsValidUri(value)) throw new global::System.ArgumentOutOfRangeException("Uri");
                __uri = (string)value;
                __has_uri = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasEvent
        {
            get
            {
                return __has_event;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __event = "";
                __has_event = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.EventType))]
        [global::System.Xml.Serialization.XmlElement("event")]
        public global::NClassify.Generator.EventType Event
        {
            get
            {
                return (global::NClassify.Generator.EventType)__event;
            }
            set
            {
                if (!IsValidEvent(value)) throw new global::System.ArgumentOutOfRangeException("Event");
                __event = (string)value;
                __has_event = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasVerified
        {
            get
            {
                return __has_verified;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __verified = false;
                __has_verified = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(false)]
        [global::System.Xml.Serialization.XmlElement("verified")]
        public bool Verified
        {
            get
            {
                return __verified;
            }
            set
            {
                __verified = value;
                __has_verified = true;
            }
        }
        #endregion
    }
    #endregion
    #region CallbackRequest
    [global::System.Xml.Serialization.XmlType("CallbackRequest")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class CallbackRequest
    {
        #region Static Data
        public static bool IsValidCallback(global::NClassify.Generator.Callback value)
        {
            if (null == value) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_callback;
        private global::NClassify.Generator.Callback __callback = new global::NClassify.Generator.Callback();
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasCallback
        {
            get
            {
                return __has_callback;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __callback = new global::NClassify.Generator.Callback();
                __has_callback = false;
            }
        }
        [global::System.Xml.Serialization.XmlElement("callback")]
        public global::NClassify.Generator.Callback Callback
        {
            get
            {
                return __callback;
            }
            set
            {
                if (!IsValidCallback(value)) throw new global::System.ArgumentOutOfRangeException("Callback");
                __callback = value;
                __has_callback = true;
            }
        }
        #endregion
    }
    #endregion
    #region CallbackVerify
    [global::System.Xml.Serialization.XmlType("CallbackVerify")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class CallbackVerify
    {
        #region Static Data
        public static bool IsValidCallbackId(global::NClassify.Generator.CallbackId testValue)
        {
            if (!testValue.HasValue) return false;
            ulong value = (ulong)testValue;
            if (!NClassify.Generator.CallbackId.IsValidValue(value)) return false;
            return true;
        }
        public static bool IsValidVerifier(string value)
        {
            if (null == value) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_callbackId;
        private ulong __callbackId;
        private bool __has_verifier;
        private string __verifier = "";
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasCallbackId
        {
            get
            {
                return __has_callbackId;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __callbackId = 0ul;
                __has_callbackId = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.CallbackId))]
        [global::System.Xml.Serialization.XmlElement("callback_id")]
        public global::NClassify.Generator.CallbackId CallbackId
        {
            get
            {
                return (global::NClassify.Generator.CallbackId)__callbackId;
            }
            set
            {
                if (!IsValidCallbackId(value)) throw new global::System.ArgumentOutOfRangeException("CallbackId");
                __callbackId = (ulong)value;
                __has_callbackId = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasVerifier
        {
            get
            {
                return __has_verifier;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __verifier = "";
                __has_verifier = false;
            }
        }
        [global::System.Xml.Serialization.XmlElement("verifier")]
        public string Verifier
        {
            get
            {
                return __verifier;
            }
            set
            {
                if (!IsValidVerifier(value)) throw new global::System.ArgumentOutOfRangeException("Verifier");
                __verifier = value;
                __has_verifier = true;
            }
        }
        #endregion
    }
    #endregion
    #region CallbackVerifyRequest
    [global::System.Xml.Serialization.XmlType("CallbackVerifyRequest")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class CallbackVerifyRequest
    {
        #region Static Data
        public static bool IsValidCallback(global::NClassify.Generator.CallbackVerify value)
        {
            if (null == value) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_callback;
        private global::NClassify.Generator.CallbackVerify __callback = new global::NClassify.Generator.CallbackVerify();
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasCallback
        {
            get
            {
                return __has_callback;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __callback = new global::NClassify.Generator.CallbackVerify();
                __has_callback = false;
            }
        }
        [global::System.Xml.Serialization.XmlElement("callback")]
        public global::NClassify.Generator.CallbackVerify Callback
        {
            get
            {
                return __callback;
            }
            set
            {
                if (!IsValidCallback(value)) throw new global::System.ArgumentOutOfRangeException("Callback");
                __callback = value;
                __has_callback = true;
            }
        }
        #endregion
    }
    #endregion
    #region Callbacks
    [global::System.Xml.Serialization.XmlType("Callbacks")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class Callbacks
    {
        #region _CallbackList
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
        partial class _CallbackList : global::System.Collections.Generic.IList<global::NClassify.Generator.Callback>
        {
            private static global::NClassify.Generator.Callback ValidateItem(global::NClassify.Generator.Callback value)
            {
                if (!IsValidCallback(value)) throw new global::System.ArgumentOutOfRangeException("Callback");
                return value;
            }
            private static global::System.Collections.Generic.IList<global::NClassify.Generator.Callback> ValidateItems(global::System.Collections.Generic.IList<global::NClassify.Generator.Callback> items)
            {
                if (null == items) throw new global::System.ArgumentNullException("Callback");
                foreach (global::NClassify.Generator.Callback item in items)
                {
                    ValidateItem(item);
                }
                return items;
            }
            private readonly bool _readOnly;
            private readonly global::System.Collections.Generic.IList<global::NClassify.Generator.Callback> _contents;
            public _CallbackList()
            {
                _readOnly = false;
                _contents = new global::System.Collections.Generic.List<global::NClassify.Generator.Callback>();
            }
            public _CallbackList(global::System.Collections.Generic.IList<global::NClassify.Generator.Callback> contents, bool readOnly)
            {
                _readOnly = readOnly;
                _contents = new global::System.Collections.Generic.List<global::NClassify.Generator.Callback>(ValidateItems(contents));
            }
            public _CallbackList AsReadOnly()
            {
                if (IsReadOnly) return this;
                return new _CallbackList(_contents, true);
            }
            private global::System.Collections.Generic.IList<global::NClassify.Generator.Callback> Modify
            {
                get { if (!IsReadOnly) return _contents; throw new global::System.InvalidOperationException(); }
            }
            public global::NClassify.Generator.Callback this[int index]
            {
                get { return _contents[index]; }
                set { Modify[index] = ValidateItem(value); }
            }
            public int Count { get { return _contents.Count; } }
            public bool IsReadOnly { get { return _readOnly || _contents.IsReadOnly; } }
            public void Add(global::NClassify.Generator.Callback item) { Modify.Add(ValidateItem(item)); }
            public void Insert(int index, global::NClassify.Generator.Callback item) { Modify.Insert(index, ValidateItem(item)); }
            public bool Remove(global::NClassify.Generator.Callback item) { return Modify.Remove(item); }
            public void RemoveAt(int index) { Modify.RemoveAt(index); }
            public void Clear() { Modify.Clear(); }
            public bool Contains(global::NClassify.Generator.Callback item) { return _contents.Contains(item); }
            public int IndexOf(global::NClassify.Generator.Callback item) { return _contents.IndexOf(item); }
            public void CopyTo(global::NClassify.Generator.Callback[] array, int arrayIndex) { _contents.CopyTo(array, arrayIndex); }
            public global::System.Collections.Generic.IEnumerator<global::NClassify.Generator.Callback> GetEnumerator()
            { return _contents.GetEnumerator(); }
            global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
            { return ((global::System.Collections.IEnumerable)_contents).GetEnumerator(); }
        }
        #endregion
        #region Static Data
        public static bool IsValidCallback(global::NClassify.Generator.Callback value)
        {
            if (null == value) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private _CallbackList __callback = new _CallbackList();
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlElement("callback")]
        public global::System.Collections.Generic.IList<global::NClassify.Generator.Callback> CallbackList
        {
            get
            {
                return __callback;
            }
            set
            {
                __callback = new _CallbackList(value, false);
            }
        }
        #endregion
    }
    #endregion
    #region CallbacksResponse
    [global::System.Xml.Serialization.XmlType("CallbacksResponse")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class CallbacksResponse
    {
        #region Static Data
        public static bool IsValidCallbacks(global::NClassify.Generator.Callbacks value)
        {
            if (null == value) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_callbacks;
        private global::NClassify.Generator.Callbacks __callbacks = new global::NClassify.Generator.Callbacks();
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasCallbacks
        {
            get
            {
                return __has_callbacks;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __callbacks = new global::NClassify.Generator.Callbacks();
                __has_callbacks = false;
            }
        }
        [global::System.Xml.Serialization.XmlElement("callbacks")]
        public global::NClassify.Generator.Callbacks Callbacks
        {
            get
            {
                return __callbacks;
            }
            set
            {
                if (!IsValidCallbacks(value)) throw new global::System.ArgumentOutOfRangeException("Callbacks");
                __callbacks = value;
                __has_callbacks = true;
            }
        }
        #endregion
    }
    #endregion
    #region CallbacksRequest
    [global::System.Xml.Serialization.XmlType("CallbacksRequest")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class CallbacksRequest
    {
        #region Static Data
        public static bool IsValidEvent(global::NClassify.Generator.EventType testValue)
        {
            if (!testValue.HasValue) return false;
            string value = (string)testValue;
            if (!NClassify.Generator.EventType.IsValidValue(value)) return false;
            return true;
        }
        public static bool IsValidUri(global::NClassify.Generator.WebUri testValue)
        {
            if (!testValue.HasValue) return false;
            string value = (string)testValue;
            if (!NClassify.Generator.WebUri.IsValidValue(value)) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_page;
        private int __page;
        private bool __has_perPage;
        private int __perPage;
        private bool __has_event;
        private string __event;
        private bool __has_uri;
        private string __uri;
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasPage
        {
            get
            {
                return __has_page;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __page = 0;
                __has_page = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(0)]
        [global::System.Xml.Serialization.XmlElement("page")]
        public int Page
        {
            get
            {
                return __page;
            }
            set
            {
                __page = value;
                __has_page = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasPerPage
        {
            get
            {
                return __has_perPage;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __perPage = 0;
                __has_perPage = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(0)]
        [global::System.Xml.Serialization.XmlElement("per_page")]
        public int PerPage
        {
            get
            {
                return __perPage;
            }
            set
            {
                __perPage = value;
                __has_perPage = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasEvent
        {
            get
            {
                return __has_event;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __event = "";
                __has_event = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.EventType))]
        [global::System.Xml.Serialization.XmlElement("event")]
        public global::NClassify.Generator.EventType Event
        {
            get
            {
                return (global::NClassify.Generator.EventType)__event;
            }
            set
            {
                if (!IsValidEvent(value)) throw new global::System.ArgumentOutOfRangeException("Event");
                __event = (string)value;
                __has_event = true;
            }
        }
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasUri
        {
            get
            {
                return __has_uri;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __uri = "";
                __has_uri = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.WebUri))]
        [global::System.Xml.Serialization.XmlElement("uri")]
        public global::NClassify.Generator.WebUri Uri
        {
            get
            {
                return (global::NClassify.Generator.WebUri)__uri;
            }
            set
            {
                if (!IsValidUri(value)) throw new global::System.ArgumentOutOfRangeException("Uri");
                __uri = (string)value;
                __has_uri = true;
            }
        }
        #endregion
    }
    #endregion
    #region CallbackIdentity
    [global::System.Xml.Serialization.XmlType("CallbackIdentity")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("NClassify.Generator", "1.0")]
    public partial class CallbackIdentity
    {
        #region Static Data
        public static bool IsValidCallbackId(global::NClassify.Generator.CallbackId testValue)
        {
            if (!testValue.HasValue) return false;
            ulong value = (ulong)testValue;
            if (!NClassify.Generator.CallbackId.IsValidValue(value)) return false;
            return true;
        }
        #endregion
        #region Instance Fields
        private bool __has_callbackId;
        private ulong __callbackId;
        #endregion
        #region Instance Members
        [global::System.Xml.Serialization.XmlIgnore()]
        public bool HasCallbackId
        {
            get
            {
                return __has_callbackId;
            }
            set
            {
                if (value) throw new global::System.InvalidOperationException();
                __callbackId = 0ul;
                __has_callbackId = false;
            }
        }
        [global::System.ComponentModel.DefaultValueAttribute(typeof(global::NClassify.Generator.CallbackId))]
        [global::System.Xml.Serialization.XmlElement("callback_id")]
        public global::NClassify.Generator.CallbackId CallbackId
        {
            get
            {
                return (global::NClassify.Generator.CallbackId)__callbackId;
            }
            set
            {
                if (!IsValidCallbackId(value)) throw new global::System.ArgumentOutOfRangeException("CallbackId");
                __callbackId = (ulong)value;
                __has_callbackId = true;
            }
        }
        #endregion
    }
    #endregion
    #region CallbackService
    #endregion
}
#endregion
