using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NClassify.Generator.CodeWriters;
using System.Text.RegularExpressions;

namespace NClassify.Generator.CodeGenerators
{
    public abstract class BaseGenerator<TWriter> where TWriter : CodeWriter
    {
        private readonly NClassifyConfig _config;

        protected BaseGenerator(NClassifyConfig config)
        {
            _config = config;
        }

        public NClassifyConfig Config { get { return _config; } }

        public virtual void GenerateCode(TextWriter writer)
        {
            using (TWriter code = OpenWriter(writer))
            {
                using (code.WriteNamespace(Config.RootNamespace.QualifiedName.Split('.', ':')))
                {
                    GenerateChildren(code, Config.RootNamespace);
                }
                CloseWriter(code);
            }
        }

        protected virtual void GenerateChildren(TWriter code, BaseType parent)
        {
            foreach (var type in Config.GetEnumerations(parent))
                GenerateEnum(code, type);
            foreach (var type in Config.GetSimpleTypes(parent))
                GenerateSimpleType(code, type);
            foreach (var type in Config.GetComplexTypes(parent))
            {
                GenerateComplexType(code, type);
                GenerateChildren(code, type);
            }
            foreach (var type in Config.GetServices(parent))
                GenerateService(code, type);
        }

        protected virtual void GenerateEnum(TWriter code, EnumType type)
        {
            using (code.DeclareEnum(Config.DefaultAccess, type.Name))
            {
                foreach (var item in type.Values)
                {
                    code.WriteEnumValue(item.Name, item.Value);
                }
            }
        }

        protected virtual void GenerateSimpleType(TWriter code, SimpleType type)
        {
            code.DeclareStruct(Config.DefaultAccess, type.Name,
                ApplyValidation(type.Validation,
                    new PropertyInfo(QualifiedTypeName(type.Type), "value")
                    {
                        Access = CodeAccess.Public,
                        ReadWrite = PropertyAccessors.Read,
                        IsValueType = IsValueType(type.Type),
                        IsClsCompliant = IsClsCompliant(type.Type),
                    }));
        }

        protected virtual void GenerateComplexType(TWriter code, ComplexType type)
        {
            List<PropertyInfo> props = new List<PropertyInfo>();
            foreach (FieldInfo field in type.Fields)
            {
                string qualifiedTypeName, pseudoType = null;
                FieldType fieldType = field.Type;
                if(field is Primitive)
                    qualifiedTypeName = QualifiedTypeName(field.Type);
                else if(field is EnumTypeRef)
                    qualifiedTypeName = QualifiedTypeName(Config.ResolveName<EnumType>(type, ((EnumTypeRef) field).TypeName));
                else if(field is SimpleTypeRef)
                {
                    SimpleType storeType = Config.ResolveName<SimpleType>(type, ((SimpleTypeRef) field).TypeName);
                    qualifiedTypeName = QualifiedTypeName(storeType.Type);
                    pseudoType = QualifiedTypeName(storeType);
                    fieldType = storeType.Type;
                }
                else if(field is ComplexTypeRef)
                    qualifiedTypeName = QualifiedTypeName(Config.ResolveName<ComplexType>(type, ((ComplexTypeRef) field).TypeName));
                else
                    throw new ApplicationException(String.Format("Unknown field type for field {0}.{1}", type.QualifiedName, field.Name));

                PropertyInfo prop = new PropertyInfo(qualifiedTypeName, field.Name)
                    {
                        Default = field.DefaultValue,
                        PseudoType = pseudoType,
                        Ordinal = field.FieldId,
                        Access = field.Access == FieldAccess.Protected ? CodeAccess.Protected
                            : field.Access == FieldAccess.Private ? CodeAccess.Private
                            : CodeAccess.Public,
                        ReadWrite = field.FieldDirection == FieldDirection.ReadOnly ? PropertyAccessors.Read
                            : field.FieldDirection == FieldDirection.WriteOnly ? PropertyAccessors.Write
                            : PropertyAccessors.ReadWrite,
                        Required = field.FieldUse == FieldUse.Required,
                        Obsolete = field.FieldUse == FieldUse.Obsolete,
                        Prohibited = field.FieldUse == FieldUse.Prohibited,
                        IsValueType = IsValueType(fieldType),
                        IsClsCompliant = IsClsCompliant(fieldType),
                    };

                props.Add(ApplyValidation(field.Validation, prop));
            }
            code.DeclareClass(Config.DefaultAccess, type.Name, null, props.ToArray());
        }

        protected virtual bool IsValueType(FieldType field)
        {
            if(field == FieldType.None ||
                field == FieldType.String ||
                field == FieldType.EMail)
                return false;

            return true;
        }

        protected virtual void GenerateService(TWriter code, ServiceInfo type)
        {
        }

        static PropertyInfo ApplyValidation(IEnumerable<ValidationRule> rules, PropertyInfo info)
        {
            if (rules == null) return info;
            foreach (ValidationRule r in rules)
            {
                if (r is LengthConstraint)
                {
                    info.MinLength = ((LengthConstraint)r).MinLengh;
                    info.MaxLength = ((LengthConstraint)r).MaxLengh;
                }
                else if (r is RangeConstraint)
                {
                    info.MinValue = ((RangeConstraint)r).MinValue;
                    info.MaxValue = ((RangeConstraint)r).MaxValue;
                }
                else if (r is MatchConstraint)
                {
                    info.Expression = ((MatchConstraint)r).Pattern;
                    info.ExpOptions =
                        (((MatchConstraint)r).IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) |
                        (((MatchConstraint)r).Multiline ? RegexOptions.Multiline : RegexOptions.Singleline);
                }
                else if (r is PredefinedValue)
                {
                    info.OneOfThese = (string[])((PredefinedValue) r).Values.Clone();
                }
                else
                    throw new ArgumentOutOfRangeException("Invalid rule type: " + r.GetType());
            }
            return info;
        }

        protected abstract TWriter OpenWriter(TextWriter writer);
        protected virtual void CloseWriter(TWriter cw) { cw.Dispose(); }

        public abstract string QualifiedTypeName(FieldType type);
        public virtual string QualifiedTypeName(EnumType type) { return QualifiedTypeName((BaseType)type); }
        public virtual string QualifiedTypeName(SimpleType type) { return QualifiedTypeName((BaseType)type); }
        public virtual string QualifiedTypeName(ComplexType type) { return QualifiedTypeName((BaseType)type); }
        public virtual string QualifiedTypeName(BaseType type) { return type.QualifiedName; }
        
        public virtual bool IsClsCompliant(FieldType type)
        {
            return type != FieldType.UInt8 && type != FieldType.UInt16 &&
                   type != FieldType.UInt32 && type != FieldType.UInt64;
        }
    }
}
