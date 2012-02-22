using System.Text.RegularExpressions;

namespace NClassify.Generator.CodeWriters
{
    public class PropertyInfo
    {
        public FieldAccess Access;
        public FieldDirection ReadWrite;
        public uint Ordinal;
        public string Type;
        public string PseudoType;
        public string Name;
        public string Default;

        public bool IsValueType;
        public bool Required;
        public bool Obsolete;
        public bool Prohibited;
        public bool IsClsCompliant;

        public uint? MinLength, MaxLength;
        public string MinValue, MaxValue;

        public RegexOptions ExpOptions;
        public string Expression;

        public string[] OneOfThese;

        public PropertyInfo(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
