using System;
using System.Collections.Generic;
using System.Linq;
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Constraints;
using System.Collections.ObjectModel;

namespace NClassify.Generator.CodeGenerators.Fields
{
    internal abstract class BaseFieldGenerator : IMemberGenerator<CsCodeWriter>
    {
        private readonly FieldInfo _field;
        private IList<BaseConstraintGenerator> _constraints;

        protected BaseFieldGenerator(FieldInfo field)
        {
            _field = field;
        }

        protected FieldInfo Field
        {
            get { return _field; }
        }

        public virtual FieldType FieldType
        {
            get { return _field.Type; }
        }

        public virtual FieldAccess FieldAccess
        {
            get { return Field.Access; }
        }

        public virtual FieldAccess ReadAccess
        {
            get { return IsWriteOnly ? FieldAccess.Private : Field.Access; }
        }

        public virtual FieldAccess WriteAccess
        {
            get { return IsReadOnly ? FieldAccess.Private : Field.Access; }
        }

        public FieldDirection Direction
        {
            get { return Field.FieldDirection; }
        }

        public bool IsReadOnly
        {
            get { return Direction == FieldDirection.ReadOnly; }
        }

        public bool IsWriteOnly
        {
            get { return Direction == FieldDirection.WriteOnly; }
        }

        public uint Ordinal
        {
            get { return Field.FieldId; }
        }

        public string Name
        {
            get { return Field.Name; }
        }

        public string CamelName
        {
            get { return CodeWriter.ToCamelCase(Field.Name); }
        }

        public string PascalName
        {
            get { return CodeWriter.ToPascalCase(Field.Name); }
        }

        public string PropertyName
        {
            get
            {
                if (Field.PropertyName != null)
                    return Field.PropertyName;
                Field.PropertyName = PascalName;
                if (Field.IsArray)
                    Field.PropertyName += "List";

                if (!ReservedWords.IsValidFieldName(Field.PropertyName))
                    Field.PropertyName += "_";

                return Field.PropertyName;
            }
        }

        public virtual string HasBackingName
        {
            get { return "__has_" + CamelName; }
        }

        public virtual string FieldBackingName
        {
            get { return "__fld_" + CamelName; }
        }

        public virtual bool HasDefault
        {
            get { return (Field.DefaultValue != null || IsNullable) && !IsStructure; }
        }

        public virtual string Default
        {
            get { return Field.DefaultValue; }
        }

        public bool Required
        {
            get { return (Field.FieldUse & FieldUse.Required) == FieldUse.Required; }
        }

        public bool Obsolete
        {
            get { return (Field.FieldUse & FieldUse.Obsolete) == FieldUse.Obsolete; }
        }

        public bool Prohibited
        {
            get { return (Field.FieldUse & FieldUse.Prohibited) == FieldUse.Prohibited; }
        }

        public bool IsStructure { get; set; }

        public virtual bool HasValidator
        {
            get { return Prohibited || Constraints.Count > 0; }
        }

        public virtual bool IsMessage
        {
            get { return false; }
        }

        public virtual bool IsArray
        {
            get { return false; }
        }

        public virtual bool IsNullable
        {
            get { return false; }
        }

        public virtual bool IsClsCompliant
        {
            get { return true; }
        }

        public virtual XmlFieldOptions XmlOptions
        {
            get 
            { 
                Field.XmlOptions = Field.XmlOptions ?? new XmlFieldOptions();
                var xdefaults = Field.DeclaringType.ParentConfig.Settings.XmlDefaults;
                if(xdefaults != null)
                {
                    if (Field.XmlOptions.AttributeType == XmlAttributeType.Default)
                        Field.XmlOptions.AttributeType = xdefaults.AttributeType;
                    Field.XmlOptions.IgnoreEmpty |= xdefaults.IgnoreEmpty;

                    foreach (var fmt in xdefaults.Formats.SafeEnum().Where(f => f.Type == FieldType))
                    {
                        if (String.IsNullOrEmpty(Field.XmlOptions.Format))
                            Field.XmlOptions.Format = fmt.Format;
                        if (Field.XmlOptions.Culture == CultureInfo.Default)
                            Field.XmlOptions.Culture = fmt.Culture;
                        Field.XmlOptions.IgnoreEmpty |= fmt.IgnoreEmpty;
                    }
                }

                if (Field.XmlOptions.AttributeType == XmlAttributeType.Default)
                    Field.XmlOptions.AttributeType = XmlAttributeType.Element;
                if (Field.XmlOptions.Culture == CultureInfo.Default)
                    Field.XmlOptions.Culture = CultureInfo.InvariantCulture;
                if (String.IsNullOrEmpty(Field.XmlOptions.Format))
                    Field.XmlOptions.Format = null;
                if (String.IsNullOrEmpty(Field.XmlOptions.XmlName))
                    Field.XmlOptions.XmlName = Field.Name;

                return Field.XmlOptions;
            }
        }

        public IList<BaseConstraintGenerator> Constraints
        {
            get { return _constraints ?? (_constraints = CreateConstraints()); }
        }

        protected virtual IList<BaseConstraintGenerator> CreateConstraints()
        {
            List<BaseConstraintGenerator> rules = new List<BaseConstraintGenerator>();
            if (IsNullable)
                rules.Add(new NotNullConstraintGenerator(this));
            rules.AddRange(ConstraintFactory.Create(this, Field.Validation));

            return new ReadOnlyCollection<BaseConstraintGenerator>(rules);
        }

        public virtual string GetStorageType(CodeWriter code)
        {
            return code.GetTypeName(Field.Type);
        }

        public virtual string GetPublicType(CodeWriter code)
        {
            return GetStorageType(code);
        }

        #region CSharp

        public virtual void MakeReadOnly(CsCodeWriter code, string value)
        { }

        public virtual string MakeConstant(CsCodeWriter code, string value)
        {
            return code.MakeConstant(FieldType, value);
        }

        public virtual bool IsPseudoTyped(CodeWriter code)
        {
            return false;
        }

        public virtual string ToStorageType(CsCodeWriter code, string valueName)
        {
            return valueName;
        }

        public virtual string ToPublicType(CsCodeWriter code, string valueName)
        {
            return valueName;
        }

        public virtual void ChecksBeforeTypeConvert(CsCodeWriter code, string valueName)
        {
        }

        public virtual void DeclareTypes(CsCodeWriter code)
        {
        }

        public virtual void DeclareStaticData(CsCodeWriter code)
        {
            Constraints.ForAll(x => x.DeclareStaticData(code));

            if (HasValidator)
            {
                bool pseudoTyped = IsPseudoTyped(code);
                using (code.WriteBlock("public static bool IsValid{0}({1} {2}, {3}System.Action<{3}NClassify.Library.ValidationError> onError)", 
                                        PropertyName, GetPublicType(code),
                                        pseudoTyped ? "testValue" : "value", CsCodeWriter.Global))
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
            code.DeclareField(new CodeItem(HasBackingName) {Access = FieldAccess.Private}, FieldType.Boolean, null);
            code.DeclareField(new CodeItem(FieldBackingName) {Access = FieldAccess.Private}, GetStorageType(code), null);
        }

        public virtual void WriteMember(CsCodeWriter code)
        {
            Constraints.ForAll(x => x.WriteMember(code));

            using (code.DeclareProperty(
                new CodeItem("Has" + PropertyName)
                    {
                        Access = FieldAccess,
                        Obsolete = Obsolete,
                        XmlName = null,
                    }, FieldType.Boolean))
            {
                code.WriteLine("get {{ return {0}; }}", HasBackingName);
                if (!IsReadOnly)
                {
                    using (code.WriteBlock("set"))
                    {
                        code.WriteLine("if (value || IsReadOnly()) throw new global::System.InvalidOperationException();");
                        code.WriteLine("{0} = {1};", FieldBackingName, MakeConstant(code, Default));
                        code.WriteLine("{0} = false;", HasBackingName);
                    }
                }
            }

            CodeItem prop = new CodeItem(PropertyName)
                                {
                                    Access = FieldAccess,
                                    //ClsCompliant = IsClsCompliant,
                                    Obsolete = Obsolete,
                                    XmlName = Name,
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
                    if (IsStructure) // Since a structure can not initalize defaults, return the constant
                        code.WriteLine("if (!{0}) return {1};", HasBackingName, MakeConstant(code, Default));
                    code.WriteLine("return {0};", ToPublicType(code, FieldBackingName));
                }
                using (code.WriteBlock(IsReadOnly && FieldAccess != FieldAccess.Private ? "private set" : "set"))
                {
                    if (IsNullable)
                        code.WriteLine("if (null == value) throw new {0}System.ArgumentNullException({1});",
                            CsCodeWriter.Global, code.MakeString(PropertyName));
                    if (!IsStructure)
                        code.WriteLine("if (IsReadOnly()) throw new {0}System.InvalidOperationException();", CsCodeWriter.Global);

                    code.WriteLine("{0} = {1};", FieldBackingName, ToStorageType(code, "value"));
                    code.WriteLine("{0} = true;", HasBackingName);
                }
            }
        }

        public virtual void WriteValidation(CsCodeWriter code)
        {
            if (HasValidator)
            {
                if (Field.FieldUse == FieldUse.Required)
                {
                    using(code.WriteBlock("if (!{0})", HasBackingName))
                    {
                        code.WriteLine("if (onError != null) " +
                            "onError(new {0}NClassify.Library.ValidationError(TypeFields.{1}, {0}NClassify.Library.ResourceMessages.MissingRequiredField, TypeFields.{1}));",
                            CsCodeWriter.Global, PropertyName);
                        code.WriteLine("errorCount++;");
                    }
                }
                else
                {
                    code.WriteLine("if ({0} && !IsValid{1}({2}, onError)) errorCount++;",
                                   HasBackingName, PropertyName, FieldBackingName);
                }
            }
        }

        public virtual void WriteClone(CsCodeWriter code)
        {
        }

        public virtual void WriteCopy(CsCodeWriter code, string from)
        {
            code.WriteLine("if ({0}.Has{1}) {1} = " + (!IsMessage ? "{0}.{1};" : "({2}){0}.{1}.Clone();"),
                from, PropertyName, GetStorageType(code) );
        }

        public virtual void WriteXmlOutput(CsCodeWriter code, string name)
        {
            if (XmlOptions.AttributeType == XmlAttributeType.Attribute)
                code.WriteLine("writer.WriteAttributeString(\"{0}\", {1});", XmlOptions.XmlName, ToXmlString(code, name));
            else if (XmlOptions.AttributeType == XmlAttributeType.Element)
            {
                code.WriteLine("writer.WriteElementString(\"{0}\", {1});", XmlOptions.XmlName, ToXmlString(code, name));
                //code.WriteLine("writer.WriteStartElement(\"{0}\");", XmlName);
                //code.WriteLine("writer.WriteString({0});", ToXmlString(code, FieldBackingName));
                //code.WriteLine("writer.WriteFullEndElement();");
            }
            else if (XmlOptions.AttributeType == XmlAttributeType.Text)
                code.WriteLine("writer.WriteString({0});", ToXmlString(code, name));
        }

        public virtual string ToXmlString(CsCodeWriter code, string name)
        {
            return String.Format("{0}NClassify.Library.TypeConverter.Instance.ToString({1}, {2}, {0}System.Globalization.CultureInfo.{3})",
                CsCodeWriter.Global, name, XmlOptions.Format == null ? "null" : code.MakeString(XmlOptions.Format), XmlOptions.Culture);
        }

        public virtual string FromXmlString(CsCodeWriter code, string name)
        {
            return String.Format("{0}NClassify.Library.TypeConverter.Instance.Parse{1}({2}, {3}, {0}System.Globalization.CultureInfo.{4})",
                CsCodeWriter.Global, FieldType, name, XmlOptions.Format == null ? "null" : code.MakeString(XmlOptions.Format), XmlOptions.Culture);
        }

        public virtual void WriteXmlInput(CsCodeWriter code)
        { }

        #endregion

        public virtual void ReadXmlMessage(CsCodeWriter code)
        {
            throw new NotSupportedException();
        }

        public virtual void ReadXmlValue(CsCodeWriter code, string value)
        {
            code.WriteLine("{0} = {1};", FieldBackingName, FromXmlString(code, value));
            if (HasBackingName != null)
                code.WriteLine(HasBackingName + " = true;");
        }
    }
}
