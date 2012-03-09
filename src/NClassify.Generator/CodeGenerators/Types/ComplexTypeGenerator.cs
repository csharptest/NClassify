#region Copyright (c) 2012 Roger O Knapp
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    class ComplexTypeGenerator : BaseTypeGenerator<ComplexType>
    {
        public ComplexTypeGenerator(ComplexType type)
            : base(type)
        { }

        public override bool IsSubclass { get { return Type.BaseClass != null; } }
        public override string VirtualApi
        {
            get { return !IsSubclass ? "public virtual" : "public override"; }
        }
        
        public override void DeclareType(CsCodeWriter code)
        {
            if (Type.Generate != CodeGeneration.ClassOnly)
                WriteInterface(code);
            if (Type.Generate != CodeGeneration.InterfaceOnly)
                WriteClass(code);
        }

        protected void WriteInterface(CsCodeWriter code)
        {
            string[] derives = new string[]
                                   {
                                       String.Format("{0}NClassify.Library.IMessage", CsCodeWriter.Global),
                                   };

            bool cls = Fields.Count(
                f => f.FieldAccess == FieldAccess.Public &&
                     f.Direction != FieldDirection.WriteOnly &&
                     f.IsClsCompliant == false) > 0;

            using (code.DeclareInterface(new CodeItem("I" + PascalName) {Access = Access}, derives))
            {
                Fields.Where(
                    f => f.FieldAccess == FieldAccess.Public && f.Direction != FieldDirection.WriteOnly
                    ).ForAll(
                    f =>
                        {
                            if (f.HasBackingName != null)
                                code.WriteLine("bool Has{0} {{ get; }}", f.PropertyName);
                            code.WriteLine("{0} {1} {{ get; }}", f.GetPublicType(code), f.PropertyName);
                        }
                    );
            }
        }

        protected void WriteClass(CsCodeWriter code)
        {
            List<string> derives = new List<string>
                                       {
                                           "I" + PascalName,
                                           String.Format("{0}NClassify.Library.IBuilder", CsCodeWriter.Global),
                                       };
            if (IsSubclass)
            {
                ComplexType type = Type.ParentConfig.ResolveName<ComplexType>(Type, Type.BaseClass);
                derives.Insert(0, CsCodeWriter.Global + type.QualifiedName);
            }

            using (code.DeclareClass(new CodeItem(PascalName) { Access = Access, XmlName = Name, }, derives.ToArray()))
            {
                code.WriteLine("private static readonly {0} _defaultInstance = new {0}(false);", PascalName);
                code.WriteLine("public static{1} {0} DefaultInstance {{ get {{ return _defaultInstance; }} }}", PascalName, IsSubclass ? " new" : "");

                using (code.WriteBlock("static {0}()", PascalName))
                {
                    code.WriteLine("_defaultInstance.Clear();");
                    code.WriteLine("_defaultInstance.MakeReadOnly();");
                }

                using (code.WriteBlock("protected {0}(bool initalize){1}", PascalName, IsSubclass ? " : base(initalize)" : ""))
                {
                    code.WriteLine("if (initalize) Initialize();");
                }

                using (code.WriteBlock("public {0}() : this(true)", PascalName))
                { }

                using (code.WriteBlock("public {0}(I{0} copyFrom) : this(true)", PascalName))
                {
                    code.WriteLine("MergeFrom(copyFrom);");
                }

                using (code.CodeRegion("TypeFields"))
                using (code.DeclareEnum(new CodeItem("TypeFields") { HidesBase = IsSubclass }))
                    Fields.ForAll(f => code.WriteLine("{0} = {1},", f.PropertyName, f.Ordinal));

                WriteChildren(code, Type.ChildTypes);
                Fields.ForAll(x => x.DeclareTypes(code));

                using (code.CodeRegion("Static Data"))
                    Fields.ForAll(x => x.DeclareStaticData(code));
                using (code.CodeRegion("Instance Fields"))
                {
                    if(!IsSubclass)
                        code.WriteLine("private bool _readOnly;");
                    Fields.ForAll(x => x.DeclareInstanceData(code));
                }
                using (code.CodeRegion("Instance Members"))
                {
                    Fields.ForAll(x => x.WriteMember(code));
                    WriteMembers(code, Fields);
                }
            }
        }

        protected override void WriteMembers(CsCodeWriter code, ICollection<BaseFieldGenerator> fields)
        {
            base.WriteMembers(code, fields);

            if (!IsSubclass)
            {
                using (code.WriteBlock("public I{0} AsReadOnly()", PascalName))
                {
                    code.WriteLine("if (_readOnly) return this;");
                    code.WriteLine("{0} copy = Clone();", PascalName);
                    code.WriteLine("copy.MakeReadOnly();");
                    code.WriteLine("return copy;");
                }
                using (code.WriteBlock("public bool IsReadOnly()"))
                {
                    code.WriteLine("return _readOnly;");
                }
            }
            using (code.WriteBlock(VirtualApi + " void MakeReadOnly()", PascalName))
            {
                if (IsSubclass)
                {
                    code.WriteLine("if (IsReadOnly()) return;");
                    code.WriteLine("base.MakeReadOnly();");
                }
                else
                {
                    code.WriteLine("if (_readOnly) return;");
                    code.WriteLine("_readOnly = true;");
                }

                fields.Where(f => !(f is ComplexFieldGenerator)).ForAll(f => f.MakeReadOnly(code, f.FieldBackingName));

                // Durring the initialization of default instance, message fields may be momentarily null.  Since
                // the static initializer for that type will correctly initialize, we will skip marking these as
                // read only.
                code.WriteLine("if (object.ReferenceEquals(this, _defaultInstance)) return;");
                fields.Where(f => f is ComplexFieldGenerator).ForAll(f => f.MakeReadOnly(code, f.FieldBackingName));
            }

            using (code.WriteBlock(VirtualApi + " void AcceptDefaults()"))
            {
                CallBase(code, "base.AcceptDefaults();");
                fields.Where(f=>f.HasBackingName != null && f.HasDefault)
                    .ForAll(f=>code.WriteLine("{0} = true;", f.HasBackingName));
            }

            using (code.WriteBlock(VirtualApi + " void Clear()"))
            {
                CallBase(code, "base.Clear();");
                code.WriteLine("Initialize();");
            }

            using (code.WriteBlock("private void Initialize()"))
            {
                foreach (var fld in fields)
                {
                    if (fld.HasBackingName != null)
                        code.WriteLine("{0} = false;", fld.HasBackingName);
                    code.WriteLine("{0} = {1};", fld.FieldBackingName, fld.MakeConstant(code, null));
                }
            }

            if (!IsSubclass)
                code.WriteLine("object {0}System.ICloneable.Clone() {{ return Clone(); }}", CsCodeWriter.Global);

            using (code.WriteBlock("protected {0} object MemberwiseClone()", IsSubclass ? "override" : "new virtual"))
            {
                code.WriteLine("{0} value = ({0})base.MemberwiseClone();", PascalName);
                foreach (var fld in fields)
                    fld.WriteClone(code);
                code.WriteLine("return value;");
            }

            using (code.WriteBlock("public{1} {0} Clone()", PascalName, IsSubclass ? " new" : ""))
            {
                code.WriteLine("return ({0})this.MemberwiseClone();", PascalName);
            }

            using (code.WriteBlock(VirtualApi + " void MergeFrom({0}NClassify.Library.IMessage other)", CsCodeWriter.Global))
            {
                code.WriteLine("if (other is I{0}) MergeFrom((I{0})other);", PascalName);
                if(!CallBase(code, "base.MergeFrom(other);"))
                {
                    code.WriteLine("else throw new global::System.ArgumentException();");
                }
            }

            using (code.WriteBlock("public void MergeFrom(I{0} other)", PascalName))
            {
                foreach (var fld in fields.Where(f => f.FieldAccess == FieldAccess.Public && !f.IsWriteOnly))
                    fld.WriteCopy(code, "other");
                CallBase(code, "base.MergeFrom(other);");
            }

            if (!IsSubclass)
            {
                using (code.WriteBlock("{0}System.Xml.Schema.XmlSchema {0}System.Xml.Serialization.IXmlSerializable.GetSchema()", CsCodeWriter.Global))
                {
                    code.WriteLine("return null;");
                }
            }

            ReaderWriterGenerator rdrwtr = new ReaderWriterGenerator(Type, XmlName, IsSubclass);
            rdrwtr.WriteXmlReadMembers(code, fields.Where(f => f.Direction != FieldDirection.WriteOnly));
            rdrwtr.WriteXmlWriteMembers(code, fields.Where(f => f.Direction != FieldDirection.ReadOnly));
        }
    }
}
