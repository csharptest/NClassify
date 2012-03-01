using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class FloatFieldGenerator : BaseNumericFieldGenerator
    {
        public FloatFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }
    }
}
