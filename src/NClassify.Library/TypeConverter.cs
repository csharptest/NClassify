#pragma warning disable 1591

namespace NClassify.Library
{
    using Regex = global::System.Text.RegularExpressions.Regex;
    using Match = global::System.Text.RegularExpressions.Match;
    using Group = global::System.Text.RegularExpressions.Group;
    using MatchCollection = global::System.Text.RegularExpressions.MatchCollection;

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
        private readonly Regex _digits = new Regex(@"^\d+");

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
            return global::System.DateTime.ParseExact(text, format, formatProvider, global::System.Globalization.DateTimeStyles.AssumeUniversal);
        }

        #region ParseTimeSpan Implementation

        private readonly global::System.Collections.Generic.Dictionary<string, object[]> _parseTimeSpanCache = 
            new global::System.Collections.Generic.Dictionary<string, object[]>(global::System.StringComparer.Ordinal);

        public virtual global::System.TimeSpan ParseTimeSpan(string text, string format, global::System.IFormatProvider formatProvider)
        {
            global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();
            int pos = 0;
            object[] numFormat;
            lock (_parseTimeSpanCache)
            {
                if (!_parseTimeSpanCache.TryGetValue(format, out numFormat))
                {
                    numFormat = new object[14];
                    global::System.Globalization.NumberFormatInfo nfi =
                        global::System.Globalization.NumberFormatInfo.GetInstance(formatProvider);
                    sb.Append("^");
                    foreach (Match m in _formatString.Matches(format))
                    {
                        sb.Append(Regex.Escape(format.Substring(pos, m.Index - pos)));
                        pos = m.Index + m.Length;
                        int fld = int.Parse(m.Groups["field"].Value);
                        if (fld < 0 || fld > 12)
                            throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
                        numFormat[fld + 1] = m.Groups["format"].Success ? m.Groups["format"].Value : null;
                        if (fld == 2) //neg sign
                            sb.AppendFormat("(?<_{0}>{1})?", fld + 1, Regex.Escape(nfi.NegativeSign));
                        else
                            sb.AppendFormat("(?<_{0}>.+?)", fld + 1);
                    }
                    sb.Append(Regex.Escape(format.Substring(pos, format.Length - pos)));
                    sb.Append("$");
                    numFormat[0] = new Regex(sb.ToString());
                    _parseTimeSpanCache[format] = numFormat;
                }
            }
            Match match = ((Regex)numFormat[0]).Match(text);
            if(!match.Success)
                throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
            
            bool negate = false;
            global::System.TimeSpan value = global::System.TimeSpan.Zero;
            
            for(int i=1; i <= 13; i++)
            {
                Group grp = match.Groups["_" + i];
                if (!grp.Success) continue;
                switch (i - 1)
                {
                    case 0: //value,
                        value += global::System.TimeSpan.Parse(grp.Value);
                        break;
                    case 1: //value.Ticks,
                        value += new global::System.TimeSpan(ParseInt64(grp.Value, (string)numFormat[i], formatProvider));
                        break;
                    case 2: //value.Ticks < 0 ? negSign : "",
                        negate = true; 
                        break;
                    case 3: //global::System.Math.Abs(value.Days),
                    case 8: //value.TotalDays,
                        value += global::System.TimeSpan.FromDays(ParseDouble(grp.Value, (string)numFormat[i], formatProvider));
                        break;
                    case 4: //global::System.Math.Abs(value.Hours),
                    case 9: //value.TotalHours,
                        value += global::System.TimeSpan.FromHours(ParseDouble(grp.Value, (string)numFormat[i], formatProvider));
                        break;
                    case 5: //global::System.Math.Abs(value.Minutes),
                    case 10: //value.TotalMinutes,
                        value += global::System.TimeSpan.FromMinutes(ParseDouble(grp.Value, (string)numFormat[i], formatProvider));
                        break;
                    case 6: //global::System.Math.Abs(value.Seconds),
                    case 11: //value.TotalSeconds,
                        value += global::System.TimeSpan.FromSeconds(ParseDouble(grp.Value, (string)numFormat[i], formatProvider));
                        break;
                    case 7: //global::System.Math.Abs(value.Milliseconds),
                    case 12: //value.TotalMilliseconds
                        value += global::System.TimeSpan.FromMilliseconds(ParseDouble(grp.Value, (string)numFormat[i], formatProvider));
                        break;
                }
            }
            return negate ? value.Negate() : value;
        }

        public virtual global::System.TimeSpan ParseTimeSpanx(string text, string format, global::System.IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return global::System.Xml.XmlConvert.ToTimeSpan(text);

            int fpos = 0;
            
            char[] chars = text.ToCharArray();
            int ixch = 0, lastch = chars.Length;

            global::System.DateTime medianDate = new global::System.DateTime(global::System.DateTime.MaxValue.Ticks >> 1);
            MatchCollection matches = _formatString.Matches(format);
            int flast = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;
            for(int ix = lastch - (format.Length - flast); flast < format.Length; ix++)
            {
                if(--lastch == 0 || chars[ix] != format[flast++])
                    throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
            }

            bool neg = false;
            int days = 0, hours = 0, minutes = 0, seconds = 0, milliseconds = 0;
            
            foreach(Match m in matches)
            {
                while(fpos < m.Index)
                    if(chars[ixch++] != format[fpos++])
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));

                fpos += m.Length;
                Match digits;

                switch (int.Parse(m.Groups["field"].Value))
                {
                    case 0: //value,
                        return global::System.TimeSpan.Parse(new string(chars, ixch, lastch - ixch));
                    case 1: //value.Ticks,
                        return new global::System.TimeSpan(
                            ParseInt64(
                                new string(chars, ixch, lastch - ixch),
                                m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                formatProvider
                            ));
                    case 2: //value.Ticks < 0 ? "-" : "",
                        string negSign = global::System.Globalization.NumberFormatInfo.GetInstance(formatProvider).NegativeSign;
                        if ((ixch + negSign.Length) < chars.Length && negSign == new string(chars, ixch, negSign.Length))
                        {
                            ixch += negSign.Length;
                            neg = true;
                        }
                        break;
                    case 3: //global::System.Math.Abs(value.Days),
                        if ((digits = _digits.Match(text, ixch, lastch - ixch)).Success)
                        {
                            ixch += digits.Length;
                            days = ParseInt32(digits.Value,
                                              m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                              formatProvider);
                            break;
                        }
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
                    case 4: //global::System.Math.Abs(value.Hours),
                        if ((digits = _digits.Match(text, ixch, lastch - ixch)).Success)
                        {
                            ixch += digits.Length;
                            hours = ParseInt32(digits.Value,
                                              m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                              formatProvider);
                            break;
                        }
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
                    case 5: //global::System.Math.Abs(value.Minutes),
                        if ((digits = _digits.Match(text, ixch, lastch - ixch)).Success)
                        {
                            ixch += digits.Length;
                            minutes = ParseInt32(digits.Value,
                                              m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                              formatProvider);
                            break;
                        }
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
                    case 6: //global::System.Math.Abs(value.Seconds),
                        if ((digits = _digits.Match(text, ixch, lastch - ixch)).Success)
                        {
                            ixch += digits.Length;
                            seconds = ParseInt32(digits.Value,
                                              m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                              formatProvider);
                            break;
                        }
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
                    case 7: //global::System.Math.Abs(value.Milliseconds),
                        if ((digits = _digits.Match(text, ixch, lastch - ixch)).Success)
                        {
                            ixch += digits.Length;
                            milliseconds = ParseInt32(digits.Value,
                                              m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                              formatProvider);
                            break;
                        }
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, text));
                    case 8: //value.TotalDays,
                        return medianDate
                            .AddDays(ParseDouble(
                                new string(chars, ixch, lastch - ixch),
                                m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                formatProvider))
                            - medianDate;
                    case 9: //value.TotalHours,
                        return medianDate
                            .AddHours(ParseDouble(
                                new string(chars, ixch, lastch - ixch),
                                m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                formatProvider))
                            - medianDate;
                    case 10: //value.TotalMinutes,
                        return medianDate
                            .AddMinutes(ParseDouble(
                                new string(chars, ixch, lastch - ixch),
                                m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                formatProvider))
                            - medianDate;
                    case 11: //value.TotalSeconds,
                        return medianDate
                            .AddSeconds(ParseDouble(
                                new string(chars, ixch, lastch - ixch),
                                m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                formatProvider))
                            - medianDate;
                    case 12: //value.TotalMilliseconds
                        return medianDate
                            .AddMilliseconds(ParseDouble(
                                new string(chars, ixch, lastch - ixch),
                                m.Groups["format"].Success ? m.Groups["format"].Value : null,
                                formatProvider))
                            - medianDate;
                    default:
                        throw new global::System.FormatException(string.Format(Resources.InvalidFormat, format));
                }
            }

            global::System.TimeSpan value = new global::System.TimeSpan(
                days, hours, minutes, seconds, milliseconds);
            return neg ? value.Negate() : value;
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