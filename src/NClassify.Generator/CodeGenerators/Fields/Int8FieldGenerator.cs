using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class Int8FieldGenerator : BaseNumericFieldGenerator
    {
        public Int8FieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsClsCompliant
        {
            get { return false; }
        }
    }
}
