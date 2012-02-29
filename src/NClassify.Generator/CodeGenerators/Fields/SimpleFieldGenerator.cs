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
        private SimpleTypeGenerator _generator;

        public SimpleFieldGenerator(FieldInfo fld)
            : base(fld)
        {
            _primitive = fld.DeclaringType.ParentConfig.ResolveName<SimpleType>(
                fld.DeclaringType, ((SimpleTypeRef) fld).TypeName
                );

            _generator = new SimpleTypeGenerator(_primitive);
        }

        public override FieldType FieldType
        {
            get
            {
                return _primitive.Type;
            }
        }

        protected override IList<BaseConstraintGenerator> CreateConstraints()
        {
            List<BaseConstraintGenerator> constraints = new List<BaseConstraintGenerator>(
                    base.CreateConstraints()
                );

            constraints.Add(new IsValidConstraintGenerator(this));
            return constraints.AsReadOnly();
        }

        public override string GetStorageType(CodeWriter code)
        {
            return CsCodeWriter.Global + _primitive.QualifiedName;
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            return String.Format("new {0}({1})", GetPublicType(code), code.MakeConstant(FieldType, value));
        }

        public override string ToXmlString(CsCodeWriter code, string name)
        {
            return _generator.ValueField.ToXmlString(code, name + ".Value");
        }

        public override string FromXmlString(CsCodeWriter code, string name)
        {
            return String.Format("new {0}({1})",
                GetPublicType(code),
                _generator.ValueField.FromXmlString(code, name)
                );
        }
    }
}
