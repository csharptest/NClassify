using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Constraints;
using NClassify.Generator.CodeGenerators.Types;

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

        public override bool IsPseudoTyped(CodeWriter code) { return true; }

        public override string ToStorageType(CsCodeWriter code, string valueName)
        {
            return String.Format("({0}){1}", GetStorageType(code), valueName);
        }
        public override string ToPublicType(CsCodeWriter code, string valueName)
        {
            return String.Format("({0}){1}", GetPublicType(code), valueName);
        }
        public override void ChecksBeforeTypeConvert(CsCodeWriter code, string valueName)
        {
            code.WriteLine("if (!{0}.HasValue) return false;", valueName);
        }

        protected override IList<BaseConstraintGenerator> CreateConstraints()
        {
            if (new SimpleTypeGenerator(_primitive).ValueField.HasValidator == false)
                return base.CreateConstraints();

            List<BaseConstraintGenerator> constraints = new List<BaseConstraintGenerator>(
                    base.CreateConstraints()
                );

            constraints.Add(new SimplyTypeConstraintGenerator(_primitive));
            return constraints.AsReadOnly();
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
