using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class GuidFieldGenerator : BaseFieldGenerator
    {
        public GuidFieldGenerator(FieldInfo fld)
            : base(fld)
        {
        }
    }
}
