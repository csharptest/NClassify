using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class ComplexFieldGenerator : BaseFieldGenerator
    {
        private ComplexType _type;

        public ComplexFieldGenerator(FieldInfo fld)
            : base(fld)
        {
            _type = fld.DeclaringType.ParentConfig.ResolveName<ComplexType>(
                fld.DeclaringType, ((ComplexTypeRef)fld).TypeName
                );
        }

        public override bool IsNullable { get { return true; } }

        public override string GetStorageType(CodeWriter code)
        {
            return CsCodeWriter.Global + _type.QualifiedName;
        }
        public override string GetPublicType(CodeWriter code)
        {
            return CsCodeWriter.Global + _type.QualifiedName;
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            if (!String.IsNullOrEmpty(value))
                throw new NotSupportedException("Complex types can not have default values.");

            return String.Format("new {0}()", CsCodeWriter.Global + _type.QualifiedName);
        }
    }
}
