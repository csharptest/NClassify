using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace NClassify.Generator
{
    [XmlRoot("NClassify")]
    public sealed partial class NClassifyConfig
    {
        [XmlElement("settings")] public NClassifySettings _settings;
        internal NClassifySettings Settings { get { return _settings ?? (_settings = new NClassifySettings()); } }

        [XmlElement("import", Type = typeof(ImportFile))]
        [XmlElement("service", Type = typeof(ServiceInfo))]
        [XmlElement("enum", Type = typeof(EnumType))]
        [XmlElement("type", Type = typeof(ComplexType))]
        [XmlElement("value", Type = typeof(SimpleType))]
        public RootItem[] Items;
    }

    public sealed class NClassifySettings
    {
        [XmlElement("namespace")]
        public string Namespace { get; set; }
    }

    public abstract partial class RootItem
    { }

    public sealed partial class ImportFile : RootItem
    {
        [XmlAttribute("file", DataType = "anyURI")]
        public string File;
    }

    public sealed class ServiceInfo : BaseType
    {
        [XmlElement("method")]
        public ServiceMethod[] Methods;
    }

    public sealed class ServiceMethod
    {
        [XmlAttribute("name", DataType = "NCName")]
        public string Name { get; set; }

        [XmlAttribute("returns", DataType = "NCName")]
        public string Response { get; set; }

        [XmlAttribute("argument", DataType = "NCName")]
        public string Request { get; set; }
    }

    public abstract partial class BaseType : RootItem
    {
        [XmlAttribute("name", DataType = "NCName")]
        public virtual string Name { get; set; }

        internal BaseType[] ChildTypes { get; set; }
    }

    public sealed class EnumType : BaseType
    {
        [XmlElement("value")]
        public Item[] Values;

        public sealed class Item
        {
            [XmlAttribute("name", DataType = "NCName")]
            public string Name;

            [XmlAttribute("ordinal")]
            public uint Value;
        }
    }

    public sealed class SimpleType : BaseType
    {
        [XmlAttribute("base")]
        public FieldType Type;

        [XmlArray("validation")]
        [XmlArrayItem("length", Type = typeof(LengthConstraint))]
        [XmlArrayItem("range", Type = typeof(RangeConstraint))]
        [XmlArrayItem("match", Type = typeof(MatchConstraint))]
        [XmlArrayItem("predefined", Type = typeof(PredefinedValue))]
        public ValidationRule[] Validation { get; set; }
    }

    public sealed class ComplexType : BaseType
    {
        [XmlArray("fields")]
        [XmlArrayItem("primitive", Type = typeof(Primitive))]
        [XmlArrayItem("complex", Type = typeof(ComplexTypeRef))]
        [XmlArrayItem("simple", Type = typeof(SimpleTypeRef))]
        [XmlArrayItem("enum", Type = typeof(EnumTypeRef))]
        public FieldInfo[] Fields;

        [XmlElement("enum", Type = typeof(EnumType))]
        [XmlElement("type", Type = typeof(ComplexType))]
        [XmlElement("value", Type = typeof(SimpleType))]
        public new BaseType[] ChildTypes { get { return base.ChildTypes; } set { base.ChildTypes = value; } }
    }

    #region Fields

    public abstract class FieldInfo
    {
        [XmlAttribute("name", DataType = "NCName")]
        public string Name { get; set; }

        [XmlIgnore]
        internal FieldType Type { get; set; }

        [XmlAttribute("property", DataType = "NCName")]
        public string PropertyName { get; set; }

        [XmlAttribute("ordinal")]
        public uint FieldId { get; set; }

        [XmlAttribute("access"), DefaultValue(FieldAccess.Public)]
        public FieldAccess Access { get; set; }

        [XmlAttribute("array"), DefaultValue(false)]
        public bool IsArray { get; set; }

        [XmlAttribute("direction"), DefaultValue(FieldDirection.Bidirectional)]
        public FieldDirection FieldDirection { get; set; }

        [XmlAttribute("use"), DefaultValue(FieldUse.Optional)]
        public FieldUse FieldUse { get; set; }

        [XmlAttribute("default"), DefaultValue(null)]
        public string DefaultValue;

        [XmlIgnore]
        internal ValidationRule[] Validation { get; set; }

        //[XmlElement("xml-options")]
        //public XmlFieldOptions XmlOptions { get; set; }
    }

    public sealed class Primitive : FieldInfo
    {
        [XmlAttribute("type")]
        public new FieldType Type { get { return base.Type; } set { base.Type = value; } }

        [XmlArray("validation")]
        [XmlArrayItem("length", Type = typeof(LengthConstraint))]
        [XmlArrayItem("range", Type = typeof(RangeConstraint))]
        [XmlArrayItem("match", Type = typeof(MatchConstraint))]
        [XmlArrayItem("predefined", Type = typeof(PredefinedValue))]
        public new ValidationRule[] Validation { get; set; }
    }

    public sealed class ComplexTypeRef : FieldInfo
    {
        public ComplexTypeRef() { Type = FieldType.None; }

        [XmlAttribute("type", DataType = "NCName")]
        public string TypeName { get; set; }
    }

    public sealed class SimpleTypeRef : FieldInfo
    {
        public SimpleTypeRef() { Type = FieldType.Value; }

        [XmlAttribute("type", DataType = "NCName")]
        public string TypeName { get; set; }
    }

    public sealed class EnumTypeRef : FieldInfo
    {
        public EnumTypeRef() { Type = FieldType.Enum; }

        [XmlAttribute("type", DataType = "NCName")]
        public string TypeName { get; set; }
    }
    
    //public sealed class XmlFieldOptions
    //{ }

    #endregion

    #region ValidationRules

    public abstract class ValidationRule { }

    public sealed class LengthConstraint : ValidationRule
    {
        [XmlAttribute("min"), DefaultValue(typeof(uint), "0")]
        public uint MinLengh { get; set; }

        [XmlAttribute("max"), DefaultValue(typeof(uint), "4294967295")]
        public uint MaxLengh { get; set; }
    }

    public sealed class RangeConstraint : ValidationRule
    {
        [XmlAttribute("min"), DefaultValue(null)]
        public string MinValue { get; set; }

        [XmlAttribute("max"), DefaultValue(null)]
        public string MaxValue { get; set; }
    }

    public sealed class MatchConstraint : ValidationRule
    {
        [XmlAttribute("pattern"), DefaultValue(".*")]
        public string Pattern { get; set; }

        [XmlAttribute("ignoreCase"), DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        [XmlAttribute("multiline"), DefaultValue(false)]
        public bool Multiline { get; set; }
    }

    public sealed class PredefinedValue : ValidationRule
    {
        [XmlElement("value")]
        public string[] Values;
    }

    #endregion

    #region Enumerations

    public enum FieldType
    {
        [XmlIgnore] None = 0,
        [XmlIgnore] Enum = 1,
        [XmlIgnore] Value = 2,
        [XmlEnum("bool")] Boolean,
        [XmlEnum("bytes")] Bytes,
        [XmlEnum("int8")] Int8,
        [XmlEnum("uint8")] UInt8,
        [XmlEnum("int16")] Int16,
        [XmlEnum("uint16")] UInt16,
        [XmlEnum("int32")] Int32,
        [XmlEnum("uint32")] UInt32,
        [XmlEnum("int64")] Int64,
        [XmlEnum("uint64")] UInt64,
        [XmlEnum("float")] Float,
        [XmlEnum("double")] Double,
        [XmlEnum("guid")] Guid,
        [XmlEnum("dateTime")] DateTime,
        [XmlEnum("timeSpan")] TimeSpan,
        [XmlEnum("string")] String,
        [XmlEnum("uri")] Uri,
        [XmlEnum("email")] EMail,
    }

    public enum FieldAccess
    {
        [XmlEnum("public")] Public,
        [XmlEnum("private")] Private,
        [XmlEnum("protected")] Protected,
    }

    public enum FieldUse
    {
        [XmlEnum("optional")] Optional,
        [XmlEnum("required")] Required,
        [XmlEnum("obsolete")] Obsolete,
        [XmlEnum("prohibited")] Prohibited,
    }

    public enum FieldDirection
    {
        [XmlEnum("read-write")] Bidirectional,
        [XmlEnum("read-only")] ReadOnly,
        [XmlEnum("write-only")] WriteOnly,
    }

    public enum GeneratorLanguage
    {
        [XmlEnum("C#")] CSharp,
    }

    #endregion
}
