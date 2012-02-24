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
                           PropertyName = fld.PropertyName,
                           Type = FieldType.Array,
                           Validation = null,
                       })
        {
            _generator = gen;
        }

        public override string HasBackingName { get { return null; } }
        public override bool HasValidator { get { return false; } }
        public override bool IsArray { get { return true; } }
        public override bool IsNullable { get { return true; } }
        public override string PropertyName { get { return base.PropertyName + "List"; } }

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
                code.WriteLine("private readonly bool _readOnly;");
                code.WriteLine("private readonly {0}System.Collections.Generic.IList<{1}> _contents;", CsCodeWriter.Global, itemType);

                using(code.WriteBlock("public {0}()", collection))
                {
                    code.WriteLine("_readOnly = false;");
                    code.WriteLine("_contents = new {0}System.Collections.Generic.List<{1}>();", CsCodeWriter.Global, itemType);
                }
                using (code.WriteBlock("public {0}({1}System.Collections.Generic.IList<{2}> contents, bool readOnly)", collection, CsCodeWriter.Global, itemType))
                {
                    code.WriteLine("_readOnly = readOnly;");
                    if (_generator.IsNullable)
                        code.WriteLine("foreach ({0} item in AssertNotNull(contents)) AssertNotNull(item);", _generator.GetPublicType(code));
                    code.WriteLine("_contents = new {0}System.Collections.Generic.List<{1}>(AssertNotNull(contents));", CsCodeWriter.Global, itemType);
                }
                _generator.Constraints.ForAll(x => x.WriteMember(code));
                using (code.WriteBlock("public {0} AsReadOnly()", collection))
                {
                    code.WriteLine("if (IsReadOnly) return this;");
                    code.WriteLine("return new {0}(_contents, true);", collection);
                }
                using (code.WriteBlock("private {0}System.Collections.Generic.IList<{1}> Modify", CsCodeWriter.Global, itemType))
                    code.WriteLine("get {{ if (!IsReadOnly) return _contents; throw new {0}System.InvalidOperationException(); }}", CsCodeWriter.Global);
                using (code.WriteBlock("public {0} this[int index]", itemType))
                {
                    code.WriteLine("get { return _contents[index]; }");
                    code.WriteLine("set {{ Modify[index] = {0}; }}", value);
                }
                code.WriteLine("public int Count { get { return _contents.Count; } }");
                code.WriteLine("public bool IsReadOnly { get { return _readOnly || _contents.IsReadOnly; } }");
                code.WriteLine("public void Add({0} value) {{ Modify.Add({1}); }}", itemType, value);
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
                    code.WriteLine("return new {0}(this, false);", collection);
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
            code.DeclareField(new CodeItem(FieldBackingName) { Access = FieldAccess.Private }, GetStorageType(code),
                !HasDefault ? null : MakeConstant(code, Default));
        }

        public override void WriteMember(CsCodeWriter code)
        {
            CodeItem prop = new CodeItem(PropertyName)
            {
                Access = _generator.FieldAccess,
                ClsCompliant = _generator.IsClsCompliant,
                Obsolete = _generator.Obsolete,
                XmlName = _generator.Name,
                XmlAttribute = _generator.XmlAttribute,
            };

            using (code.DeclareProperty(prop, GetPublicType(code)))
            {
                using (code.WriteBlock(IsWriteOnly && FieldAccess != FieldAccess.Private ? "private get" : "get"))
                {
                    if (IsReadOnly)
                        code.WriteLine("return new {0}System.Collections.ObjectModel.ReadOnlyCollection<{1}>({2});", 
                            CsCodeWriter.Global, _generator.GetPublicType(code), FieldBackingName);
                    else
                        code.WriteLine("return {0};", FieldBackingName);
                }
                using (code.WriteBlock(IsReadOnly && FieldAccess != FieldAccess.Private ? "private set" : "set"))
                {
                    code.WriteLine("{0} = new {1}(value, false);", FieldBackingName, GetStorageType(code));
                }
            }
        }

        public override void WriteValidation(CsCodeWriter code)
        {
            if (_generator.HasValidator)
            {
                using(code.WriteBlock("foreach ({0} item in {1})", _generator.GetPublicType(code), FieldBackingName))
                    code.WriteLine("if (!IsValid{0}(item)) return false;", _generator.PropertyName);
            }
        }

        public override void WriteClone(CsCodeWriter code)
        {
            code.WriteLine("value.{0} = value.{0}.Clone();", FieldBackingName);
        }

        public override void WriteXmlOutput(CsCodeWriter code, string name)
        {
            using (code.WriteBlock("foreach ({0} item in {1})", _generator.GetPublicType(code), name))
                _generator.WriteXmlOutput(code, "item");
        }
    }
}
