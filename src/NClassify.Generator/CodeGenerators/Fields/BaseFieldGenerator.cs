﻿using System;
using System.Collections.Generic;
using System.Linq;
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Constraints;
using System.Collections.ObjectModel;

namespace NClassify.Generator.CodeGenerators.Fields
{
    abstract class BaseFieldGenerator : IMemberGenerator<CsCodeWriter>
    {
        private readonly FieldInfo _field;
        private IList<BaseConstraintGenerator> _constraints;

        protected BaseFieldGenerator(FieldInfo field)
        {
            _field = field;
            XmlAttribute = XmlAttributeType.Element;
        }

        protected FieldInfo Field { get { return _field; } }
        public virtual FieldType FieldType { get { return _field.Type; } }

        public virtual FieldAccess FieldAccess { get { return Field.Access; } }
        public virtual FieldAccess ReadAccess { get { return IsWriteOnly ? FieldAccess.Private : Field.Access; } }
        public virtual FieldAccess WriteAccess { get { return IsReadOnly ? FieldAccess.Private : Field.Access; } }

        public FieldDirection Direction { get { return Field.FieldDirection; } }
        public bool IsReadOnly { get { return Direction == FieldDirection.ReadOnly; } }
        public bool IsWriteOnly { get { return Direction == FieldDirection.WriteOnly; } }

        public uint Ordinal { get { return Field.FieldId; } }

        public string Name { get { return Field.Name; } }
        public string CamelName { get { return CodeWriter.ToCamelCase(Field.Name); } }
        public string PascalName { get { return CodeWriter.ToPascalCase(Field.Name); } }
        public virtual string PropertyName { get { return Field.PropertyName ?? PascalName; } }

        public string HasBackingName { get { return "__has_" + CamelName; } }
        public string FieldBackingName { get { return "__" + CamelName; } }

        public virtual bool HasDefault { get { return (Field.DefaultValue != null || IsNullable) && !IsStructure; } }
        public virtual string Default { get { return Field.DefaultValue; } }

        public bool Required { get { return (Field.FieldUse & FieldUse.Required) == FieldUse.Required; } }
        public bool Obsolete { get { return (Field.FieldUse & FieldUse.Obsolete) == FieldUse.Obsolete; } }
        public bool Prohibited { get { return (Field.FieldUse & FieldUse.Prohibited) == FieldUse.Prohibited; } }

        public bool IsStructure { get; set; }
        public XmlAttributeType XmlAttribute { get; set; }

        public virtual bool HasValidator { get { return Prohibited || Constraints.Count > 0; } }
        public virtual bool IsArray { get { return false; } }
        public virtual bool IsNullable { get { return false; } }
        public virtual bool IsNumeric { get { return false; } }
        public virtual bool IsUnsigned { get { return false; } }
        public virtual bool IsClsCompliant { get { return !IsNumeric || !IsUnsigned; } }

        public IList<BaseConstraintGenerator> Constraints
        {
            get { return _constraints ?? (_constraints = CreateConstraints()); }
        }

        protected virtual IList<BaseConstraintGenerator> CreateConstraints()
        {
            List<BaseConstraintGenerator> rules = new List<BaseConstraintGenerator>();
            if (IsNullable)
                rules.Add(new NotNullConstraintGenerator());
            rules.AddRange(ConstraintFactory.Create(this, Field.Validation));

            return new ReadOnlyCollection<BaseConstraintGenerator>(rules);
        }

        public virtual string GetStorageType(CodeWriter code)
        {
            return code.GetTypeName(Field.Type);
        }

        public virtual string GetPublicType(CodeWriter code)
        {
            return code.GetTypeName(Field.Type);
        }

        #region CSharp

        public virtual string MakeConstant(CsCodeWriter code, string value)
        {
            return code.MakeConstant(FieldType, value);
        }

        public virtual bool IsPseudoTyped(CodeWriter code) { return false; }
        public virtual string ToStorageType(CsCodeWriter code, string valueName)
        { return valueName; }
        public virtual string ToPublicType(CsCodeWriter code, string valueName)
        { return valueName; }
        public virtual void ChecksBeforeTypeConvert(CsCodeWriter code, string valueName)
        { }

        public virtual void DeclareTypes(CsCodeWriter code)
        { }

        public virtual void DeclareStaticData(CsCodeWriter code)
        {
            Constraints.ForAll(x => x.DeclareStaticData(code));

            if (HasValidator)
            {
                bool pseudoTyped = IsPseudoTyped(code);
                using (code.WriteBlock("public static bool IsValid{0}({1} {2})", PropertyName, GetPublicType(code),
                                    pseudoTyped ? "testValue" : "value"))
                {
                    if (Prohibited)
                        code.WriteLine("throw new " + CsCodeWriter.Global + "System.InvalidOperationException();");
                    else
                    {
                        if (pseudoTyped)
                        {
                            ChecksBeforeTypeConvert(code, "testValue");
                            code.WriteLine("{0} value = {1};", GetStorageType(code), ToStorageType(code, "testValue"));
                        }
                        Constraints.ForAll(x => x.WriteChecks(code));
                        code.WriteLine("return true;");
                    }
                }
            }
        }

        public virtual void DeclareInstanceData(CsCodeWriter code)
        {
            code.DeclareField(new CodeItem(HasBackingName) { Access = FieldAccess.Private }, FieldType.Boolean, null);
            code.DeclareField(new CodeItem(FieldBackingName) { Access = FieldAccess.Private }, GetStorageType(code), 
                !HasDefault ? null : MakeConstant(code, Default));
        }

        public virtual void WriteMember(CsCodeWriter code)
        {
            Constraints.ForAll(x => x.WriteMember(code));

            using (code.DeclareProperty(
                new CodeItem("Has" +  PropertyName)
                    {
                        Access = FieldAccess,
                        Obsolete = Obsolete,
                        XmlName = null,
                        XmlAttribute = XmlAttributeType.Ignore,
                    }, FieldType.Boolean))
            {
                using (code.WriteBlock("get"))
                    code.WriteLine("return {0};", HasBackingName);
                if (!IsReadOnly)
                {
                    using (code.WriteBlock("set"))
                    {
                        code.WriteLine("if (value) throw new global::System.InvalidOperationException();");
                        code.WriteLine("{0} = {1};", FieldBackingName, MakeConstant(code, Default));
                        code.WriteLine("{0} = false;", HasBackingName);
                    }
                }
            }

            CodeItem prop = new CodeItem(PropertyName)
                                {
                                    Access = FieldAccess,
                                    ClsCompliant = IsClsCompliant,
                                    Obsolete = Obsolete,
                                    XmlName = Name,
                                    XmlAttribute = XmlAttribute,
                                };
            if (Field.FieldUse != FieldUse.Required && !IsNullable)
            {
                if (Field.Type == FieldType.Simple)
                    prop.DefaultValue = String.Format("typeof({0})", GetPublicType(code));
                else
                    prop.DefaultValue = MakeConstant(code, Default);
            }

            using (code.DeclareProperty(prop, GetPublicType(code)))
            {
                using (code.WriteBlock(IsWriteOnly && FieldAccess != FieldAccess.Private ? "private get" : "get"))
                {
                    if (IsReadOnly && Default == null)
                        code.WriteLine("if (!{0}) throw new global::System.InvalidOperationException();", HasBackingName);

                    code.WriteLine("return {0};", ToPublicType(code, FieldBackingName));
                }
                using (code.WriteBlock(IsReadOnly && FieldAccess != FieldAccess.Private ? "private set" : "set"))
                {
                    if (HasValidator)
                        code.WriteLine("if (!IsValid{0}(value)) throw new {1}System.ArgumentOutOfRangeException({2});", 
                            PropertyName, CsCodeWriter.Global, code.MakeString(PropertyName));

                    code.WriteLine("{0} = {1};", FieldBackingName, ToStorageType(code, "value"));
                    code.WriteLine("{0} = true;", HasBackingName);
                }
            }
        }
        #endregion
    }
}
