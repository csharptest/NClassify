#pragma warning disable 1591
namespace NClassify.Library
{
    public struct ByteArray : global::System.IComparable, global::System.IFormattable, global::System.ICloneable,
        global::System.IComparable<ByteArray>, global::System.IEquatable<ByteArray>,
        global::System.IComparable<byte[]>, global::System.IEquatable<byte[]>,
        global::System.Collections.Generic.IList<byte>
    {
        public static readonly ByteArray Empty;
        #region Private Static
        private static readonly int[] Crc32Table;
        private static readonly int[] HexValues;
        private static readonly byte[] HexChars;
        static ByteArray()
        {
            Empty = new ByteArray(new byte[0]);
            Crc32Table = new int[256];
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (uint j = 0; j < 8; j++)
                    crc = (crc >> 1) ^ (((crc & 1) == 1) ? 0xEDB88320u : 0);
                Crc32Table[~i & 0x0ff] = ~unchecked((int)crc);
            }
            HexChars = global::System.Text.Encoding.UTF8.GetBytes("0123456789abcdef");
            HexValues = new int[103];
            for (int i = 0; i < 10; i++) HexValues['0' + i] = i;
            for (int i = 0; i < 6; i++) HexValues['a' + i] = 10 + i;
            for (int i = 0; i < 6; i++) HexValues['A' + i] = 10 + i;
        }

        /// <summary> Compares the contents of the byte arrays and returns the result. </summary>
        public static bool Equals(byte[] ar1, byte[] ar2)
        {
            if (ar1 == ar2) return true;
            if (ar1 == null || ar2 == null) return false;
            if (ar1.Length != ar2.Length) return false;
            return 0 == Compare(ar1, ar2);
        }

        /// <summary> Compares the contents of the byte arrays and returns the result. </summary>
        public static int Compare(byte[] ar1, byte[] ar2)
        {
            if (ar1 == null) return ar2 == null ? 0 : -1;
            if (ar2 == null) return 1;

            int result = 0;
            int i = 0, stop = global::System.Math.Min(ar1.Length, ar2.Length);

            for (; 0 == result && i < stop; i++)
                result = ar1[i].CompareTo(ar2[i]);

            if (result != 0)
                return result;
            if (i == ar1.Length)
                return i == ar2.Length ? 0 : -1;
            return 1;
        }
        #endregion

        private readonly byte[] _value;

        private ByteArray(byte[] bytes, bool copy)
        {
            if(bytes != null && copy)
                bytes = (byte[]) bytes.Clone();
            _value = bytes;
        }

        public ByteArray(byte[] bytes)
            : this(bytes, true)
        {
            if (bytes == null)
                throw new global::System.ArgumentNullException("bytes");
        }

        public int Length { get { return _value == null ? 0 : _value.Length; } }

        private byte[] SafeBytes { get { return _value ?? Empty._value; } }
        private global::System.Collections.Generic.IList<byte> ByteList { get { return SafeBytes; } }

        public byte[] ToArray()
        {
            return (byte[]) SafeBytes.Clone();
        }

        public byte[] ToArray(int offset, int length)
        {
            if (offset + length > Length)
                throw new global::System.ArgumentOutOfRangeException("length");
            byte[] copy = new byte[length];
            global::System.Buffer.BlockCopy(SafeBytes, offset, copy, 0, length);
            return copy;
        }

        public global::System.IO.Stream ToStream()
        { return ToStream(0, Length); }

        public global::System.IO.Stream ToStream(int offset, int length)
        {
            return new global::System.IO.MemoryStream(SafeBytes, offset, length, false, false);
        }

        object global::System.ICloneable.Clone()
        { return this; }

        #region Operators
        public static implicit operator ByteArray(byte[] value)
        {
            return new ByteArray(value);
        }
        public static explicit operator byte[](ByteArray value)
        {
            return value.ToArray();
        }
        public static bool operator ==(ByteArray x, ByteArray y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(ByteArray x, ByteArray y)
        {
            return !x.Equals(y);
        }
        public static bool operator ==(byte[] x, ByteArray y)
        {
            return y.Equals(x);
        }
        public static bool operator !=(byte[] x, ByteArray y)
        {
            return !y.Equals(x);
        }
        public static bool operator ==(ByteArray x, byte[] y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(ByteArray x, byte[] y)
        {
            return !x.Equals(y);
        }
        #endregion
        #region Copy To/From and Read/Write

        public void CopyTo(byte[] array, int arrayIndex)
        {
            global::System.Buffer.BlockCopy(SafeBytes, 0, array, arrayIndex, Length);
        }

        public void CopyTo(int sourceIndex, byte[] destinationArray, int destinationIndex, int length)
        {
            global::System.Buffer.BlockCopy(SafeBytes, sourceIndex, destinationArray, destinationIndex, length);
        }

        public void WriteTo(global::System.IO.Stream stream) { WriteTo(stream, 0, Length); }
        public void WriteTo(global::System.IO.Stream stream, int offset, int count)
        {
            stream.Write(SafeBytes, offset, count);
        }

        public static ByteArray CopyFrom(byte[] array, int arrayIndex, int length)
        {
            if (array == null)
                throw new global::System.ArgumentNullException("array");
            if (arrayIndex + length > array.Length)
                throw new global::System.ArgumentOutOfRangeException("length");
            byte[] bytes = new byte[length];
            global::System.Buffer.BlockCopy(array, arrayIndex, bytes, 0, length);
            return new ByteArray(bytes, false);
        }

        public static ByteArray ReadFrom(global::System.IO.Stream stream, int count)
        {
            int len, pos = 0;
            byte[] bytes;
            if (count <= 0x10000)
            {
                bytes = new byte[count];
                while (count > pos && (len = stream.Read(bytes, pos, count - pos)) > 0)
                    pos += len;
            }
            else
            {
                bytes = new byte[0x10000];
                using (global::System.IO.MemoryStream ms = new global::System.IO.MemoryStream(0x10000))
                {
                    while (count > pos && (len = stream.Read(bytes, 0, global::System.Math.Min(0x10000, count - pos))) > 0)
                    {
                        ms.Write(bytes, 0, len);
                        pos += len;
                    }
                    bytes = ms.GetBuffer();
                    global::System.Array.Resize(ref bytes, count);
                }
            }
            if (pos != count)
                throw new global::System.IO.EndOfStreamException();
            return new ByteArray(bytes, false);
        }

        #endregion
        #region Equality and Comparison
        public override int GetHashCode()
        {
            int crc32 = 0;
            for (int i = 0; i < Length; i++)
                crc32 = (((~crc32 >> 8) & 0x00FFFFFF) ^ Crc32Table[(crc32 ^ _value[i]) & 0x0ff]);
            return crc32;
        }

        public override bool Equals(object value)
        {
            if (value is ByteArray)
                return Equals(((ByteArray)value).SafeBytes);
            return Equals(value as byte[]);
        }
        public bool Equals(ByteArray value) { return Equals(value.SafeBytes); }
        public bool Equals(byte[] obj) { return Equals(SafeBytes, obj); }


        int global::System.IComparable.CompareTo(object value)
        {
            if(value is ByteArray)
                return CompareTo(((ByteArray)value).SafeBytes);
            return CompareTo(value as byte[]);
        }
        public int CompareTo(ByteArray value) { return CompareTo(value.SafeBytes); }
        public int CompareTo(byte[] value) { return Compare(SafeBytes, value); }
        #endregion
        #region To/From String
        public override string ToString()
        { return global::System.Convert.ToBase64String(SafeBytes); }

        public string ToString(string format, global::System.IFormatProvider formatProvider)
        {
            if (format == "x" || string.IsNullOrEmpty(format))
                return ToHex();
            if (format == "b")
                return ToBase64();
            throw new global::System.ArgumentOutOfRangeException("format", "Invalid format specifier.");
        }

        public static ByteArray Parse(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (format == "x" || string.IsNullOrEmpty(format))
                return FromHex(text);
            if (format == "b")
                return FromBase64(text);
            throw new global::System.ArgumentOutOfRangeException("format", "Invalid format specifier.");
        }

        public string ToUtf8()
        { return global::System.Text.Encoding.UTF8.GetString(SafeBytes); }

        public string ToUtf8(int offset, int length)
        { return global::System.Text.Encoding.UTF8.GetString(SafeBytes, offset, length); }

        public static ByteArray FromUtf8(string text)
        { return global::System.Text.Encoding.UTF8.GetBytes(text); }

        public static ByteArray FromUtf8(string text, int offset, int length)
        { return global::System.Text.Encoding.UTF8.GetBytes(text.ToCharArray(), offset, length); }

        public string ToBase64()
        { return global::System.Convert.ToBase64String(SafeBytes); }

        public string ToBase64(global::System.Base64FormattingOptions options)
        { return global::System.Convert.ToBase64String(SafeBytes, options); }

        public string ToBase64(int offset, int length)
        { return global::System.Convert.ToBase64String(SafeBytes, offset, length); }

        public static ByteArray FromBase64(string s)
        {
            return new ByteArray(global::System.Convert.FromBase64String(s), false);
        }

        public string ToHex() { return ToHex(0, Length); }
        public string ToHex(int offset, int length)
        {
            int pos = 0;
            char[] output = new char[length * 2];
            byte[] input = SafeBytes;
            for (int ix = offset; ix < (offset + length); ix++)
            {
                byte b = input[ix];
                output[pos++] = (char)(HexChars[(b >> 4) & 0x0f]);
                output[pos++] = (char)(HexChars[b & 0x0f]);
            }
            return new string(output);
        }

        public static ByteArray FromHex(string input) { return FromHex(input, 0, input.Length); }
        public static ByteArray FromHex(string input, int offset, int length)
        {
            int pos = 0;
            int end = offset + length;
            int i = offset;
            byte[] results = new byte[length / 2];
            while (i < end)
            {
                byte ch1 = (byte)input[i++];
                if (char.IsWhiteSpace((char)ch1) || ch1 == '-')
                    continue;
                if (i >= input.Length)
                    throw new global::System.FormatException();
                byte ch2 = (byte)input[i++];

                if (((ch1 >= '0' && ch1 <= '9') || (ch1 >= 'a' && ch1 <= 'f') || (ch1 >= 'A' && ch1 <= 'F')) &&
                    ((ch2 >= '0' && ch2 <= '9') || (ch2 >= 'a' && ch2 <= 'f') || (ch2 >= 'A' && ch2 <= 'F')))
                    results[pos++] = (byte)(HexValues[ch1] << 4 | HexValues[ch2]);
                else
                    throw new global::System.FormatException();
            }
            if (pos != results.Length)
                global::System.Array.Resize(ref results, pos);
            return new ByteArray(results, false);
        }
        #endregion
        #region IList<byte> Members

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index > Length)
                    throw new global::System.ArgumentOutOfRangeException("index");
                return SafeBytes[index];
            }
            set { throw new global::System.NotSupportedException(); }
        }

        int global::System.Collections.Generic.IList<byte>.IndexOf(byte item)
        {
            return ByteList.IndexOf(item);
        }

        bool global::System.Collections.Generic.ICollection<byte>.Contains(byte item)
        {
            return ByteList.Contains(item);
        }

        int global::System.Collections.Generic.ICollection<byte>.Count
        {
            get { return Length; }
        }

        bool global::System.Collections.Generic.ICollection<byte>.IsReadOnly
        {
            get { return true; }
        }

        void global::System.Collections.Generic.IList<byte>.Insert(int index, byte item)
        {
            throw new global::System.NotSupportedException();
        }

        void global::System.Collections.Generic.IList<byte>.RemoveAt(int index)
        {
            throw new global::System.NotSupportedException();
        }

        void global::System.Collections.Generic.ICollection<byte>.Add(byte item)
        {
            throw new global::System.NotSupportedException();
        }

        void global::System.Collections.Generic.ICollection<byte>.Clear()
        {
            throw new global::System.NotSupportedException();
        }

        bool global::System.Collections.Generic.ICollection<byte>.Remove(byte item)
        {
            throw new global::System.NotSupportedException();
        }

        public global::System.Collections.Generic.IEnumerator<byte> GetEnumerator()
        { return ByteList.GetEnumerator(); }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        { return SafeBytes.GetEnumerator(); }

        #endregion
    }

    public partial class TypeConverter
    {
        public virtual ByteArray ParseBytes(string text, string format, global::System.IFormatProvider formatProvider)
        {
            return ByteArray.Parse(text, format, formatProvider);
        }

        public virtual string ToString(ByteArray value, string format, global::System.IFormatProvider formatProvider)
        {
            return value.ToString(format, formatProvider);
        }
    }
}
