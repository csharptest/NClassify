﻿#region Copyright (c) 2012 Roger O Knapp
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace NClassify.Generator
{
    [XmlRoot("NClassify")]
    public sealed partial class NClassifyConfig
    {
        [XmlElement("settings")] public NClassifySettings _settings;
        internal NClassifySettings Settings { get { return _settings ?? (_settings = new NClassifySettings()); } }

        [XmlElement("import", Type = typeof(ImportFile))]
        [XmlElement("message", Type = typeof(ComplexType))]
        [XmlElement("value", Type = typeof(SimpleType))]
        [XmlElement("enum", Type = typeof(EnumType))]
        [XmlElement("service", Type = typeof(ServiceInfo))]
        public RootItem[] Items;
    }

    public sealed class NClassifySettings
    {
        [XmlAttribute("src")]
        public string IncludeSettingsFile { get; set; }

        [XmlElement("namespace")]
        public string Namespace { get; set; }

        [XmlElement("xml-defaults")]
        public XmlDefaults XmlDefaults { get; set; }
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

        [XmlAttribute("access"), DefaultValue(FieldAccess.Default)]
        public FieldAccess Access { get; set; }

        [XmlIgnore]
        internal FieldInfo[] Fields { get; set; }
        [XmlIgnore]
        internal BaseType[] ChildTypes { get; set; }
    }

    public sealed class EnumType : BaseType
    {
        [XmlElement("value")]
        public Item[] Values { get; set; }

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
        [XmlArrayItem("code", Type = typeof(CodedConstraint))]
        public ValidationRule[] Validation { get; set; }
    }

    public partial class ComplexType : BaseType
    {
        [XmlAttribute("inherits", DataType = "NCName")]
        public string BaseClass { get; set; }

        [XmlAttribute("generate"), DefaultValue(CodeGeneration.Default)]
        public CodeGeneration Generate { get; set; }

        [XmlArray("implements")]
        [XmlArrayItem("interface")]
        public string[] Interfaces { get; set; }

        [XmlArray("fields")]
        [XmlArrayItem("primitive", Type = typeof(Primitive))]
        [XmlArrayItem("message", Type = typeof(ComplexTypeRef))]
        [XmlArrayItem("value", Type = typeof(SimpleTypeRef))]
        [XmlArrayItem("enum", Type = typeof(EnumTypeRef))]
        public new FieldInfo[] Fields { get { return base.Fields; } set { base.Fields = value; } }

        [XmlElement("message", Type = typeof(ComplexType))]
        [XmlElement("value", Type = typeof(SimpleType))]
        [XmlElement("enum", Type = typeof(EnumType))]
        public new BaseType[] ChildTypes { get { return base.ChildTypes; } set { base.ChildTypes = value; } }
    }

    #region Fields

    public abstract partial class FieldInfo
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

        [XmlElement("xml-options")]
        public XmlFieldOptions XmlOptions { get; set; }
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
        [XmlArrayItem("code", Type = typeof(CodedConstraint))]
        public new ValidationRule[] Validation { get { return base.Validation; } set { base.Validation = value; } }
    }

    public sealed class ComplexTypeRef : FieldInfo
    {
        public ComplexTypeRef() { Type = FieldType.Complex; }

        [XmlAttribute("type", DataType = "NCName")]
        public string TypeName { get; set; }
    }

    public sealed class SimpleTypeRef : FieldInfo
    {
        public SimpleTypeRef() { Type = FieldType.Simple; }

        [XmlAttribute("type", DataType = "NCName")]
        public string TypeName { get; set; }
    }

    public sealed class EnumTypeRef : FieldInfo
    {
        public EnumTypeRef() { Type = FieldType.Enum; }

        [XmlAttribute("type", DataType = "NCName")]
        public string TypeName { get; set; }
    }

    public sealed class XmlDefaults
    {
        [XmlAttribute("namespace-uri")]
        public string NamespaceUri { get; set; }
        [XmlAttribute("form"), DefaultValue(XmlAttributeType.Element)]
        public XmlAttributeType AttributeType { get; set; }
        [XmlAttribute("ignore-empty"), DefaultValue(false)]
        public bool IgnoreEmpty { get; set; }

        [XmlElement("formatting")]
        public FormatInfo[] Formats { get; set; }

        public struct FormatInfo
        {
            [XmlAttribute("type")]
            public FieldType Type { get; set; }
            [XmlAttribute("format")]
            public string Format { get; set; }
            [XmlAttribute("culture"), DefaultValue(CultureInfo.InvariantCulture)]
            public CultureInfo Culture { get; set; }
            [XmlAttribute("ignore-empty"), DefaultValue(false)]
            public bool IgnoreEmpty { get; set; }
        }
    }

    public sealed class XmlFieldOptions
    {
        [XmlAttribute("form"), DefaultValue(XmlAttributeType.Default)]
        public XmlAttributeType AttributeType { get; set; }
        [XmlAttribute("name", DataType = "NCName")]
        public string XmlName { get; set; }
        [XmlAttribute("array-item-name", DataType = "NCName")]
        public string NestedArrayItemName { get; set; }
        [XmlAttribute("format")]
        public string Format { get; set; }
        [XmlAttribute("culture"), DefaultValue(CultureInfo.Default)]
        public CultureInfo Culture { get; set; }
        [XmlAttribute("ignore-empty"), DefaultValue(false)]
        public bool IgnoreEmpty { get; set; }
    }

    #endregion

    #region ValidationRules

    public abstract class ValidationRule { }

    public sealed class LengthConstraint : ValidationRule
    {
        [XmlAttribute("min"), DefaultValue(typeof(uint), "0")]
        public uint MinLength { get; set; }

        [XmlAttribute("max"), DefaultValue(typeof(uint), "4294967295")]
        public uint MaxLength { get; set; }
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

    public sealed class CodedConstraint : ValidationRule
    {
        [XmlAttribute("language")]
        public GeneratorLanguage Language;
        [XmlAttribute("test")]
        public string Code;
        [XmlElement("method"), DefaultValue(null)]
        public string MethodBody;
    }

    #endregion

    #region Enumerations

    public enum FieldType
    {
        [XmlIgnore, Obsolete] Undefined = 0,
        [XmlIgnore] Array = 2,
        [XmlIgnore] Complex = 3,
        [XmlIgnore] Enum = 4,
        [XmlIgnore] Simple = 5,
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
    }

    public enum FieldAccess
    {
        [XmlIgnore] Default = 0,
        [XmlEnum("public")] Public = 1,
        [XmlEnum("private")] Private = 2,
        [XmlEnum("protected")] Protected = 3,
    }

    [Flags]
    public enum FieldUse
    {
        [XmlEnum("optional")] Optional = 0,
        [XmlEnum("required")] Required = 1,
        [XmlEnum("obsolete")] Obsolete = 2,
        [XmlEnum("prohibited")] Prohibited = 4,
    }

    public enum FieldDirection
    {
        [XmlEnum("read-write")] Bidirectional = 0,
        [XmlEnum("read-only")] ReadOnly = 1,
        [XmlEnum("write-only")] WriteOnly = 2,
    }

    public enum GeneratorLanguage
    {
        [XmlEnum("C#")] CSharp,
    }

    public enum XmlAttributeType
    {
        [XmlIgnore] Default = 0,
        [XmlEnum("ignore")] Ignore,
        [XmlEnum("element")] Element,
        [XmlEnum("attribute")] Attribute,
        [XmlEnum("text")] Text,
    }

    public enum CultureInfo
    {
        [XmlIgnore] Default = 0,
        [XmlEnum("invariant")] InvariantCulture = 1,
        [XmlEnum("current")] CurrentCulture,
        [XmlEnum("current-ui")] CurrentUICulture,
        [XmlEnum("installed-ui")] InstalledUICulture,
    }

    public enum CodeGeneration
    {
        [XmlEnum("all")] Default = 0,
        [XmlEnum("interface-only")] InterfaceOnly = 1,
        [XmlEnum("class-only")] ClassOnly = 2,
    }

    #endregion
}
