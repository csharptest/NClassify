﻿using System;
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
        private readonly ComplexType _type;
        private readonly ComplexType _impl;

        public ComplexFieldGenerator(FieldInfo fld)
            : base(fld)
        {
            _impl = _type = fld.DeclaringType.ParentConfig.ResolveName<ComplexType>(
                fld.DeclaringType, ((ComplexTypeRef)fld).TypeName
                );

            if (_type is InterfaceType)
                _impl = ((InterfaceType) _type).Implementation;
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

        public string GetImplementationType(CsCodeWriter code)
        {
            return CsCodeWriter.Global + _type.ImplementationName;
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            if (!String.IsNullOrEmpty(value))
                throw new NotSupportedException("Complex types can not have default values.");

            return String.Format("{0}.DefaultInstance", CsCodeWriter.Global + _type.ImplementationName);
        }

        public override void MakeReadOnly(CsCodeWriter code, string value)
        {
            if (_type.ImplementationName != _type.QualifiedName)
            {
                code.WriteLine("{0} = ({0} as {1}{2}) ?? new {1}{2}({0});", value, CsCodeWriter.Global, _type.ImplementationName);
                code.WriteLine("(({1}{2}){0}).MakeReadOnly();", value, CsCodeWriter.Global, _type.ImplementationName);
            }
            else
                code.WriteLine("{0}.MakeReadOnly();", value);
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
            code.WriteLine("value.{0} = ({1})value.{0}.Clone();", FieldBackingName, GetStorageType(code));
        }

        public override void ReadXmlMessage(CsCodeWriter code)
        {
            if (_type.ImplementationName != _type.QualifiedName)
                code.WriteLine("{0}{1} value = ({2} as {0}{1}) ?? new {0}{1}({2});",
                    CsCodeWriter.Global, _type.ImplementationName, FieldBackingName);
            else
                code.WriteLine("{0}{1} value = {2};", 
                    CsCodeWriter.Global, _type.ImplementationName, FieldBackingName);

            code.WriteLine("if (value.IsReadOnly())");
            code.WriteLineIndent("value = object.ReferenceEquals(value, {0}{1}.DefaultInstance) ? new {0}{1}() : new {0}{1}({2});",
                CsCodeWriter.Global, _type.ImplementationName, FieldBackingName);

            code.WriteLine("value.ReadXml(reader.LocalName, reader);");
            code.WriteLine("{0} = value;", FieldBackingName);
            if (HasBackingName != null)
                code.WriteLine(HasBackingName + " = true;");
        }

        public override void WriteXmlOutput(CsCodeWriter code, string name)
        {
            code.WriteLine("{0}.WriteXml(\"{1}\", writer);", name, XmlOptions.XmlName);
        }
    }
}
