using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class UnsignedFieldGenerator : PrimitiveFieldGenerator
    {
        public UnsignedFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsClsCompliant
        {
            get { return false; }
        }
    }
}
