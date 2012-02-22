using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NClassify.Generator.CodeGenerators.Fields
{
    abstract class BaseNumericFieldGenerator : BaseFieldGenerator
    {
        protected BaseNumericFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }

        public override bool IsNumeric { get { return true; } }
    }
}
