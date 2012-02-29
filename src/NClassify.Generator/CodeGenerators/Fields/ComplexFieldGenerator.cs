using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeGenerators.Constraints;
using NClassify.Generator.CodeGenerators.Types;
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

        public override bool IsMessage { get { return true; } }
        public override bool IsNullable { get { return true; } }
        
        public override string GetStorageType(CodeWriter code)
        {
            return CsCodeWriter.Global + _type.QualifiedName;
        }

        public override XmlFieldOptions XmlOptions
        {
            get
            {
                XmlFieldOptions options =  base.XmlOptions;
                options.AttributeType = XmlAttributeType.Element;
                return options;
            }
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            if (!String.IsNullOrEmpty(value))
                throw new NotSupportedException("Complex types can not have default values.");

            return String.Format("new {0}()", CsCodeWriter.Global + _type.QualifiedName);
        }

        protected override IList<BaseConstraintGenerator> CreateConstraints()
        {
            List<BaseConstraintGenerator> constraints = new List<BaseConstraintGenerator>(
                    base.CreateConstraints()
                );

            constraints.Add(new IsValidConstraintGenerator(this));
            return constraints.AsReadOnly();
        }

        public override void WriteClone(CsCodeWriter code)
        {
            code.WriteLine("value.{0} = value.{0}.Clone();", FieldBackingName);
        }

        public override void WriteXmlOutput(CsCodeWriter code, string name)
        {
            code.WriteLine("{0}.WriteXml(\"{1}\", writer);", name, XmlOptions.XmlName);
        }
    }
}
