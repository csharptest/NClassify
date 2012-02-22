using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NClassify.Generator.CodeGenerators.Fields
{
    abstract class BaseUnsignedFieldGenerator : BaseNumericFieldGenerator
    {
        protected BaseUnsignedFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsUnsigned { get { return true; } }
    }
}
