using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class DateTimeFieldGenerator : BaseFieldGenerator
    {
        public DateTimeFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }
    }
}
