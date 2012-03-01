#pragma warning disable 1591, 3021

namespace NClassify.Library
{
    using Regex = global::System.Text.RegularExpressions.Regex;
    using Match = global::System.Text.RegularExpressions.Match;
    using Group = global::System.Text.RegularExpressions.Group;

    public partial class TypeConverter
    {
        private static TypeConverter _instance = new TypeConverter();

        public static TypeConverter Instance
        {
            get { return _instance; }
            set
            {
                if (value == null)
                    throw new global::System.ArgumentNullException("value");
                _instance = value;
            }
        }

        private readonly Regex _stdNumberFormat = new Regex(@"[a-zA-Z]\d*");
        private readonly Regex _formatString = new Regex(@"(?<!{){(?<field>\d+)(?<suffix>(?:,(?<width>-?\d+))?(?:\:(?<format>[^}{]+))?)}");

        #region Convert From Text
        
        protected virtual global::System.Globalization.NumberStyles NumberFormatToStyle(string format)
        {
            char stfmtId = _stdNumberFormat.IsMatch(format) ? char.ToUpper(format[0]) : '\0';
            switch (stfmtId)
            {
                case 'C':
                    return global::System.Globalization.NumberStyles.AllowLeadingSign |
                            global::System.Globalization.NumberStyles.AllowThousands |
                            global::System.Globalization.NumberStyles.AllowDecimalPoint |
                            global::System.Globalization.NumberStyles.AllowTrailingSign;
                case 'P':
                    return global::System.Globalization.NumberStyles.AllowLeadingSign |
                            global::System.Globalization.NumberStyles.AllowDecimalPoint |
                            global::System.Globalization.NumberStyles.AllowTrailingSign;
                case 'D':
                    return global::System.Globalization.NumberStyles.AllowLeadingSign;
                case 'E':
                    return global::System.Globalization.NumberStyles.AllowExponent |
                            global::System.Globalization.NumberStyles.AllowDecimalPoint |
                            global::System.Globalization.NumberStyles.AllowLeadingSign;
                case 'F':
                case 'R':
                    return global::System.Globalization.NumberStyles.AllowDecimalPoint |
                            global::System.Globalization.NumberStyles.AllowLeadingSign;
                case 'G':
                    return global::System.Globalization.NumberStyles.Any;
                case 'N':
                    return global::System.Globalization.NumberStyles.AllowDecimalPoint |
                            global::System.Globalization.NumberStyles.AllowLeadingSign |
                            global::System.Globalization.NumberStyles.AllowThousands |
                            global::System.Globalization.NumberStyles.AllowTrailingSign;
                case 'X':
                    return global::System.Globalization.NumberStyles.AllowHexSpecifier;
                default:
                    throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
            }
        }

        public virtual bool ParseBoolean(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                if (global::System.StringComparer.OrdinalIgnoreCase.Equals(bool.TrueString, text))
                    return true;
                if (global::System.StringComparer.OrdinalIgnoreCase.Equals(bool.FalseString, text))
                    return false;
                return global::System.Xml.XmlConvert.ToBoolean(text);
            }
            //allows: "1|0", "yes|no", "TRUE|FALSE", or combiniations "1|0;yes|no;true|false"
            if (format.IndexOf('|') > 0)
            {
                foreach (string pair in format.Split(';', ','))
                {
                    string[] values = pair.Split('|');
                    if (values.Length == 2 && global::System.StringComparer.OrdinalIgnoreCase.Equals(values[0], text))
                        return true;
                    if (values.Length == 2 && global::System.StringComparer.OrdinalIgnoreCase.Equals(values[1], text))
                        return false;
                }
                throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
            }
            throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
        }

        [global::System.CLSCompliant(false)]
        public virtual sbyte ParseInt8(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                int value = global::System.Xml.XmlConvert.ToInt16(text);
                if (value < sbyte.MinValue || value > sbyte.MaxValue)
                    throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
                return (sbyte) value;
            }
            return sbyte.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual byte ParseUInt8(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                ushort value = global::System.Xml.XmlConvert.ToUInt16(text);
                if (value > byte.MaxValue)
                    throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
                return (byte)value;
            }
            return byte.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual short ParseInt16(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToInt16(text);
            return short.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        [global::System.CLSCompliant(false)]
        public virtual ushort ParseUInt16(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToUInt16(text);
            return ushort.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual int ParseInt32(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToInt32(text);
            return int.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        [global::System.CLSCompliant(false)]
        public virtual uint ParseUInt32(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToUInt32(text);
            return uint.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual long ParseInt64(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToInt64(text);
            return long.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        [global::System.CLSCompliant(false)]
        public virtual ulong ParseUInt64(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToUInt64(text);
            return ulong.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual float ParseFloat(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToSingle(text);
            return float.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual double ParseDouble(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToDouble(text);
            return double.Parse(text, NumberFormatToStyle(format), formatProvider);
        }

        public virtual global::System.Guid ParseGuid(string text, string format, global::System.IFormatProvider formatProvider)
        {
            return new global::System.Guid(text);
        }

        public virtual global::System.DateTime ParseDateTime(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToDateTime(text, global::System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
            return global::System.DateTime.ParseExact(text, format, formatProvider, global::System.Globalization.DateTimeStyles.None);
        }

        #region ParseTimeSpan Implementation

        private readonly global::System.Collections.Generic.Dictionary<string, object[]> _parseTimeSpanCache = 
            new global::System.Collections.Generic.Dictionary<string, object[]>(global::System.StringComparer.Ordinal);

        private object[] CreateExpression(out Regex exp, out int[] fieldsUsed, string format, global::System.IFormatProvider formatProvider)
        {
            global::System.Globalization.NumberFormatInfo nfi =
                global::System.Globalization.NumberFormatInfo.GetInstance(formatProvider);

            object[] numFormat;
            lock (_parseTimeSpanCache)
            {
                if (!_parseTimeSpanCache.TryGetValue(format, out numFormat))
                {
                    numFormat = new object[15];
                    global::System.Collections.Generic.List<int> fields = new global::System.Collections.Generic.List<int>();

                    int pos = 0;
                    global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();
                    sb.Append("^");
                    foreach (Match m in _formatString.Matches(format))
                    {
                        sb.Append(Regex.Escape(format.Substring(pos, m.Index - pos)));
                        pos = m.Index + m.Length;
                        int fld = int.Parse(m.Groups["field"].Value);
                        if (fld < 0 || fld > 12)
                            throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
                        if (fld == 2) //neg sign
                            sb.AppendFormat("(?<f{0}>{1})?", fld, Regex.Escape(nfi.NegativeSign));
                        else if(pos+1 < format.Length)
                            sb.AppendFormat("(?<f{0}>[^{1}]+?)", fld, Regex.Escape(new string(format[pos + 1], 1)));
                        else
                            sb.AppendFormat("(?<f{0}>.+)", fld);
                        numFormat[fld] = m.Groups["format"].Success ? m.Groups["format"].Value : null;
                        fields.Add(fld);
                    }
                    sb.Append(Regex.Escape(format.Substring(pos, format.Length - pos)));
                    sb.Append("$");
                    numFormat[13] = new Regex(sb.ToString());
                    numFormat[14] = fields.ToArray();
                    _parseTimeSpanCache[format] = numFormat;
                }
            }
            exp = (Regex) numFormat[13];
            fieldsUsed = (int[]) numFormat[14];
            return numFormat;
        }

        public virtual global::System.TimeSpan ParseTimeSpan(string text, string format, global::System.IFormatProvider formatProvider)
        {
            Regex pattern;
            int[] fields;
            object[] numFormat = CreateExpression(out pattern, out fields, format, formatProvider);
            Match match = pattern.Match(text);
            if(!match.Success)
                throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
            
            bool negate = false;
            global::System.TimeSpan value = global::System.TimeSpan.Zero;
            
            foreach(int ixfld in fields)
            {
                Group grp = match.Groups["f" + ixfld];
                if (!grp.Success) continue;
                switch (ixfld)
                {
                    case 0: //value,
                        value += global::System.TimeSpan.Parse(grp.Value);
                        break;
                    case 1: //value.Ticks,
                        value += new global::System.TimeSpan(ParseInt64(grp.Value, (string)numFormat[ixfld], formatProvider));
                        break;
                    case 2: //value.Ticks < 0 ? negSign : "",
                        negate = true; 
                        break;
                    case 3: //global::System.Math.Abs(value.Days),
                    case 8: //value.TotalDays,
                        value += global::System.TimeSpan.FromDays(ParseDouble(grp.Value, (string)numFormat[ixfld], formatProvider));
                        break;
                    case 4: //global::System.Math.Abs(value.Hours),
                    case 9: //value.TotalHours,
                        value += global::System.TimeSpan.FromHours(ParseDouble(grp.Value, (string)numFormat[ixfld], formatProvider));
                        break;
                    case 5: //global::System.Math.Abs(value.Minutes),
                    case 10: //value.TotalMinutes,
                        value += global::System.TimeSpan.FromMinutes(ParseDouble(grp.Value, (string)numFormat[ixfld], formatProvider));
                        break;
                    case 6: //global::System.Math.Abs(value.Seconds),
                    case 11: //value.TotalSeconds,
                        value += global::System.TimeSpan.FromSeconds(ParseDouble(grp.Value, (string)numFormat[ixfld], formatProvider));
                        break;
                    case 7: //global::System.Math.Abs(value.Milliseconds),
                    case 12: //value.TotalMilliseconds
                        value += global::System.TimeSpan.FromMilliseconds(ParseDouble(grp.Value, (string)numFormat[ixfld], formatProvider));
                        break;
                }
            }
            return negate ? value.Negate() : value;
        }
        #endregion

        #endregion
        #region Convert To Text

        public virtual string ToString(bool value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            //allows: "1|0", "yes|no", "TRUE|FALSE", or combiniations "1|0;yes|no;true|false"
            if (format.IndexOf('|') > 0) 
                return format.Split('|', ';', ',')[value ? 0 : 1];
            throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
        }

        [global::System.CLSCompliant(false)]
        public virtual string ToString(sbyte value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(byte value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(short value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        [global::System.CLSCompliant(false)]
        public virtual string ToString(ushort value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(int value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        [global::System.CLSCompliant(false)]
        public virtual string ToString(uint value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(long value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        [global::System.CLSCompliant(false)]
        public virtual string ToString(ulong value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(float value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(double value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(global::System.Guid value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(global::System.DateTime value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value, global::System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
            return value.ToString(format, formatProvider);
        }

        public virtual string ToString(global::System.TimeSpan value, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToString(value);
            //ex: "{2}{3} {4:d2}:{5:d2}:{6:d2}.{7:d3}ms" == "-1 20:13:49.100ms"
            //ex: "{11:n3} seconds" == "-98,983.542 seconds"
            string negSign = global::System.Globalization.NumberFormatInfo.GetInstance(formatProvider).NegativeSign;
            return string.Format(
                formatProvider, format,
                //{0}-{2}
                value, value.Ticks, value.Ticks < 0 ? negSign : "",
                //{3}-{7}
                global::System.Math.Abs(value.Days), global::System.Math.Abs(value.Hours),
                global::System.Math.Abs(value.Minutes), global::System.Math.Abs(value.Seconds),
                global::System.Math.Abs(value.Milliseconds),
                //{8}-{12}
                value.TotalDays, value.TotalHours,
                value.TotalMinutes, value.TotalSeconds,
                value.TotalMilliseconds
                );
        }

        #endregion
    }
}