#pragma warning disable 1591

namespace NClassify.Generator
{
    internal class System
    {
    }

    partial class CallbacksSample
    {
        public partial class CallbackList : global::System.Collections.Generic.IList<Callback>
        {
            public static readonly CallbackList Empty = new CallbackList(new Callback[0], true);

            public static bool IsValidItem(Callback value) { return false; }
            private static Callback ValidateItem(Callback value)
            {
                if (!IsValidItem(value))
                    throw new global::System.ArgumentOutOfRangeException("Item");
                return value;
            }

            private readonly bool _readOnly;
            private readonly global::System.Collections.Generic.IList<Callback> _contents;

            public CallbackList()
            {
                _readOnly = false;
                _contents = new global::System.Collections.Generic.List<Callback>();
            }
            public CallbackList(global::System.Collections.Generic.IList<Callback> contents, bool readOnly)
            {
                _readOnly = readOnly;
                _contents = new global::System.Collections.Generic.List<Callback>(contents);
            }
            public CallbackList AsReadOnly()
            {
                if (IsReadOnly) return this;
                return new CallbackList(_contents, true);
            }
            private global::System.Collections.Generic.IList<Callback> Modify
            {
                get { if (!IsReadOnly) return _contents; throw new global::System.InvalidOperationException(); }
            }
            public Callback this[int index]
            {
                get { return _contents[index]; }
                set { Modify[index] = ValidateItem(value); }
            }
            public int Count { get { return _contents.Count; } }
            public bool IsReadOnly { get { return _readOnly || _contents.IsReadOnly; } }
            public void Add(Callback item) { Modify.Add(ValidateItem(item)); }
            public void Insert(int index, Callback item) { Modify.Insert(index, ValidateItem(item)); }
            public bool Remove(Callback item) { return Modify.Remove(item); }
            public void RemoveAt(int index) { Modify.RemoveAt(index); }
            public void Clear() { Modify.Clear(); }
            public bool Contains(Callback item) { return _contents.Contains(item); }
            public int IndexOf(Callback item) { return _contents.IndexOf(item); }
            public void CopyTo(Callback[] array, int arrayIndex) { _contents.CopyTo(array, arrayIndex); }
            public global::System.Collections.Generic.IEnumerator<Callback> GetEnumerator()
            { return _contents.GetEnumerator(); }
            global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
            { return ((global::System.Collections.IEnumerable)_contents).GetEnumerator(); }
        }
    }
}
