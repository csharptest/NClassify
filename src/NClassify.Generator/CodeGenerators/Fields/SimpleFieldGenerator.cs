using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class SimpleFieldGenerator : BaseFieldGenerator
    {
        private SimpleType _primitive;

        public SimpleFieldGenerator(FieldInfo fld)
            : base(fld)
        {
            _primitive = fld.DeclaringType.ParentConfig.ResolveName<SimpleType>(
                fld.DeclaringType, ((SimpleTypeRef) fld).TypeName
                );
        }

        public override FieldType FieldType
        {
            get
            {
                return _primitive.Type;
            }
        }

        public override string GetStorageType(CodeWriter code)
        {
            return code.GetTypeName(_primitive.Type);
        }

        public override string GetPublicType(CodeWriter code)
        {
            return CsCodeWriter.Global + _primitive.QualifiedName;
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            return code.MakeConstant(FieldType, value);
        }
    }
}
