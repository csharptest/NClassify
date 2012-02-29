using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class UInt16FieldGenerator : BaseUnsignedFieldGenerator
    {
        public UInt16FieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsClsCompliant
        {
            get { return false; }
        }
    }
}
