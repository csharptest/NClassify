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
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class ArrayFieldGenerator : BaseFieldGenerator
    {
        private readonly BaseFieldGenerator _generator;

        public ArrayFieldGenerator(FieldInfo fld, BaseFieldGenerator gen)
            : base(new Primitive()
                       {
                           Access = fld.Access,
                           DeclaringType = fld.DeclaringType,
                           DefaultValue = null,
                           FieldDirection = fld.FieldDirection,
                           FieldId = fld.FieldId,
                           FieldUse = fld.FieldUse,
                           IsArray = false,
                           Name = fld.Name,
                           XmlOptions = fld.XmlOptions,
                           PropertyName = gen.PropertyName,
                           Type = FieldType.Array,
                           Validation = null,
                       })
        {
            _generator = gen;
        }

        public override bool IsMessage
        {
            get { return _generator.IsMessage; }
        }
        public override string HasBackingName { get { return null; } }
        public override bool HasValidator { get { return false; } }
        public override bool IsArray { get { return true; } }
        public override bool IsNullable { get { return true; } }

        public override XmlFieldOptions XmlOptions
        {
            get
            {
                var options = base.XmlOptions;
                options.AttributeType = XmlAttributeType.Element;
                return options;
            }
        }

        public override string GetStorageType(CodeWriter code)
        {
            return "_" + PropertyName;
        }

        public override string GetPublicType(CodeWriter code)
        {
            return CsCodeWriter.Global + "System.Collections.Generic.IList<" + _generator.GetPublicType(code) + ">";
        }

        public override string MakeConstant(CsCodeWriter code, string value)
        {
            if (!String.IsNullOrEmpty(value))
                throw new NotSupportedException("Unable to define a default value for an array.");
            return "new " + GetStorageType(code) + "()";
        }

        public override void MakeReadOnly(CsCodeWriter code, string value)
        {
            code.WriteLine("{0}.MakeReadOnly();", value);
        }

        public override void DeclareTypes(CsCodeWriter code)
        {
            string collection = GetStorageType(code);
            string itemType = _generator.GetPublicType(code);
            
            using (code.CodeRegion(collection))
            using (code.DeclareClass(
                new CodeItem(collection) { Access = FieldAccess.Private },
                new[]
                    {
                        CsCodeWriter.Global + "System.Collections.Generic.IList<" + _generator.GetPublicType(code) + ">",
                        CsCodeWriter.Global + "System.ICloneable",
                    }))
            {
                using(code.WriteBlock("private static T AssertNotNull<T>(T value) where T : class"))
                {
                    code.WriteLine("if (null == value) throw new {0}System.ArgumentNullException({1});",
                        CsCodeWriter.Global, code.MakeString(PropertyName)); 
                    code.WriteLine("return value;");
                }

                string value = _generator.IsNullable ? "AssertNotNull(value)" : "value";
                code.WriteLine("private bool _readOnly;");
                code.WriteLine("private readonly {0}System.Collections.Generic.List<{1}> _contents;", CsCodeWriter.Global, itemType);

                using(code.WriteBlock("public {0}()", collection))
                {
                    code.WriteLine("_readOnly = false;");
                    code.WriteLine("_contents = new {0}System.Collections.Generic.List<{1}>();", CsCodeWriter.Global, itemType);
                }
                using (code.WriteBlock("public {0}({1}System.Collections.Generic.IList<{2}> contents, bool clone)", collection, CsCodeWriter.Global, itemType))
                {
                    code.WriteLine("_readOnly = false;");
                    if (!_generator.IsNullable)
                    {
                        code.WriteLine("_contents = new {0}System.Collections.Generic.List<{1}>(AssertNotNull(contents));",
                            CsCodeWriter.Global, itemType);
                    }
                    else
                    {
                        code.WriteLine("_contents = new {0}System.Collections.Generic.List<{1}>(AssertNotNull(contents).Count);",
                            CsCodeWriter.Global, itemType);
                        using (code.WriteBlock("foreach ({0} item in contents)", _generator.GetPublicType(code)))
                        {
                            if (_generator.IsMessage)
                            {
                                code.WriteLine("if (clone)");
                                code.WriteLineIndent("_contents.Add(({0})AssertNotNull(item).Clone());", itemType);
                                code.WriteLine("else");
                            }
                            code.WriteLine("_contents.Add(AssertNotNull(item));");
                        }
                    }
                }
                _generator.Constraints.ForAll(x => x.WriteMember(code));
                using (code.WriteBlock("public void MakeReadOnly()"))
                {
                    code.WriteLine("if (_readOnly) return;");
                    code.WriteLine("_readOnly = true;");
                    if (_generator.IsMessage)
                        using (code.WriteBlock("for (int i=0; i < _contents.Count; i++)"))
                        {
                            _generator.MakeReadOnly(code, "_contents[i]");
                        }
                }
                using (code.WriteBlock("private {0}System.Collections.Generic.List<{1}> Modify", CsCodeWriter.Global, itemType))
                    code.WriteLine("get {{ if (!IsReadOnly) return _contents; throw new {0}System.InvalidOperationException(); }}", CsCodeWriter.Global);
                using (code.WriteBlock("public {0} this[int index]", itemType))
                {
                    code.WriteLine("get { return _contents[index]; }");
                    code.WriteLine("set {{ Modify[index] = {0}; }}", value);
                }
                code.WriteLine("public int Count { get { return _contents.Count; } }");
                code.WriteLine("public bool IsReadOnly { get { return _readOnly; } }");
                code.WriteLine("public void Add({0} value) {{ Modify.Add({1}); }}", itemType, value);
                using (code.WriteBlock("public void AddRange({1}System.Collections.Generic.ICollection<{0}> value)", itemType, CsCodeWriter.Global))
                {
                    if (_generator.IsNullable)
                        code.WriteLine("foreach ({0} item in AssertNotNull(value)) AssertNotNull(item);", _generator.GetPublicType(code));
                    code.WriteLine("Modify.AddRange(AssertNotNull(value));");
                }
                code.WriteLine("public void Insert(int index, {0} value) {{ Modify.Insert(index, {1}); }}", itemType, value);
                code.WriteLine("public bool Remove({0} item) {{ return Modify.Remove(item); }}", itemType);
                code.WriteLine("public void RemoveAt(int index) { Modify.RemoveAt(index); }");
                code.WriteLine("public void Clear() { Modify.Clear(); }");
                code.WriteLine("public bool Contains({0} item) {{ return _contents.Contains(item); }}", itemType);
                code.WriteLine("public int IndexOf({0} item) {{ return _contents.IndexOf(item); }}", itemType);
                code.WriteLine("public void CopyTo({0}[] array, int arrayIndex) {{ _contents.CopyTo(array, arrayIndex); }}", itemType);
                code.WriteLine("object {0}System.ICloneable.Clone() {{ return Clone(); }}", CsCodeWriter.Global);
                using (code.WriteBlock("public {0} Clone()", collection))
                {
                    code.WriteLine("return _readOnly ? this : new {0}(this, true);", collection);
                }
                code.WriteLine("public {0}System.Collections.Generic.IEnumerator<{1}> GetEnumerator()", CsCodeWriter.Global, itemType);
                code.WriteLine("{ return _contents.GetEnumerator(); }");
                code.WriteLine("{0}System.Collections.IEnumerator {0}System.Collections.IEnumerable.GetEnumerator()", CsCodeWriter.Global);
                code.WriteLine("{{ return (({0}System.Collections.IEnumerable)_contents).GetEnumerator(); }}", CsCodeWriter.Global);
            }
        }

        public override void DeclareStaticData(CsCodeWriter code)
        {
            _generator.DeclareStaticData(code);
        }

        public override void DeclareInstanceData(CsCodeWriter code)
        {
            code.DeclareField(new CodeItem(FieldBackingName) { Access = FieldAccess.Private }, GetStorageType(code), null);
        }

        public override void WriteMember(CsCodeWriter code)
        {
            CodeItem prop = new CodeItem(PropertyName)
            {
                Access = _generator.FieldAccess,
                //ClsCompliant = _generator.IsClsCompliant,
                Obsolete = _generator.Obsolete,
                XmlName = _generator.Name,
            };

            using (code.DeclareProperty(prop, GetPublicType(code)))
            {
                using (code.WriteBlock(IsWriteOnly && FieldAccess != FieldAccess.Private ? "private get" : "get"))
                {
                    if (IsReadOnly)
                        code.WriteLine("if (!IsReadOnly()) return new {0}System.Collections.ObjectModel.ReadOnlyCollection<{1}>({2});", 
                            CsCodeWriter.Global, _generator.GetPublicType(code), FieldBackingName);

                    code.WriteLine("return {0};", FieldBackingName);
                }
                using (code.WriteBlock(IsReadOnly && FieldAccess != FieldAccess.Private ? "private set" : "set"))
                {
                    code.WriteLine("if (IsReadOnly()) throw new {0}System.InvalidOperationException();", CsCodeWriter.Global);
                    code.WriteLine("{0} = new {1}(value, false);", FieldBackingName, GetStorageType(code));
                }
            }
        }

        public override void WriteValidation(CsCodeWriter code)
        {
            if (_generator.HasValidator)
            {
                using(code.WriteBlock("foreach ({0} item in {1})", _generator.GetPublicType(code), FieldBackingName))
                    code.WriteLine("if (!IsValid{0}(item, onError)) errorCount++;", _generator.PropertyName);
            }
        }

        public override void WriteClone(CsCodeWriter code)
        {
            code.WriteLine("value.{0} = value.{0}.Clone();", FieldBackingName);
        }

        public override void WriteCopy(CsCodeWriter code, string other)
        {
            if(_generator.IsMessage)
            {
                code.WriteLine("foreach ({0} item in {1}.{2})", _generator.GetPublicType(code), other, PropertyName);
                code.WriteLineIndent("{0}.Add(({1})item.Clone());", FieldBackingName, _generator.GetStorageType(code));
            }
            else
                code.WriteLine("{0}.AddRange({1}.{2});", FieldBackingName, other, PropertyName);
        }

        public override void WriteXmlOutput(CsCodeWriter code, string name)
        {
            using (code.WriteBlock("foreach ({0} item in {1})", _generator.GetPublicType(code), name))
                _generator.WriteXmlOutput(code, "item");
        }

        public override void ReadXmlMessage(CsCodeWriter code)
        {
            if (_generator.IsMessage)
                code.WriteLine("{0} child = new {0}();", ((ComplexFieldGenerator)_generator).GetImplementationType(code));
            else
                code.WriteLine("{0} child = {1};", _generator.GetPublicType(code), _generator.MakeConstant(code, null));

            code.WriteLine("(({0}NClassify.Library.IBuilder)child).ReadXml(reader.LocalName, reader);", CsCodeWriter.Global);
            code.WriteLine("{0}.Add(child);", FieldBackingName);
        }

        public override void ReadXmlValue(CsCodeWriter code, string value)
        {
            code.WriteLine("{0}.Add({1});", FieldBackingName, _generator.FromXmlString(code, value));
        }
    }
}
