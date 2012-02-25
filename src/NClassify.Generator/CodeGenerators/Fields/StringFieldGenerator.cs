using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class StringFieldGenerator : BaseFieldGenerator
    {
        public StringFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsNullable { get { return true; } }

        public override string ToXmlString(CsCodeWriter code, string name)
        {
            return name;
        }

        public override string FromXmlString(CsCodeWriter code, string name)
        {
            return name;
        }
    }
}
