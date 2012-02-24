using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class BytesFieldGenerator : BaseFieldGenerator
    {
        public BytesFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsNullable { get { return true; } }

        public override void WriteClone(CsCodeWriter code)
        {
            code.WriteLine("value.{0} = ({1})value.{0}.Clone();", FieldBackingName, GetStorageType(code));
        }
    }
}
