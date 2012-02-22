using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class StringFieldGenerator : BaseFieldGenerator
    {
        public StringFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsNullable { get { return true; } }
    }
}
