using System;

namespace NClassify.Generator.CodeWriters
{
    public class CodeItem
    {
        public CodeItem(string name)
        {
            Name = name;
            Access = FieldAccess.Public;
            ClsCompliant = true;
        }
        public string Name;
        public FieldAccess Access;
        public string XmlName;
        public string Description;

        public string DefaultValue;

        public bool Obsolete;
        public bool ClsCompliant;
        public bool HidesBase;
    }
}