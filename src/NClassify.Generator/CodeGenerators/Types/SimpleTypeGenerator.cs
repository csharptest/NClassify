using System;
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Fields;

namespace NClassify.Generator.CodeGenerators.Types
{
    class SimpleTypeGenerator : BaseTypeGenerator<SimpleType>
    {
        public SimpleTypeGenerator(SimpleType type)
            : base(type)
        { }

        public BaseFieldGenerator ValueField
        {
            get
            {
                BaseFieldGenerator field = FieldFactory.Create(this,
                    new Primitive()
                        {
                            Access = FieldAccess.Public,
                            FieldDirection = FieldDirection.ReadOnly,
                            FieldId = 1,
                            FieldUse = FieldUse.Required,
                            Name = "value",
                            Type = Type.Type,
                            Validation = Type.Validation,
                            DefaultValue = null,
                        });

                field.IsStructure = true;
                return field;
            }
        }

        public override void DeclareType(CsCodeWriter code)
        {
            BaseFieldGenerator field = ValueField;

            string[] derives = new[]
                                   {
                                       String.Format("{0}System.IEquatable<{1}>", CsCodeWriter.Global, PascalName),
                                       String.Format("{0}System.IComparable<{1}>", CsCodeWriter.Global, PascalName),
                                       String.Format("{0}NClassify.Library.IValidate", CsCodeWriter.Global),
                                   };

            using (code.DeclareStruct(
                new CodeItem(PascalName)
                    {
                        Access = Access,
                        XmlName = Name,
                    },
                    derives))
            {
                using (code.CodeRegion("TypeFields"))
                using (code.DeclareEnum(new CodeItem("TypeFields")))
                    code.WriteLine("Value = 0");

                field.DeclareTypes(code);

                using (code.CodeRegion("Instance Fields and Members"))
                {
                    field.DeclareStaticData(code);
                    field.DeclareInstanceData(code);

                    code.SetClsCompliant(field.IsClsCompliant);
                    using (code.WriteBlock("public {0}({1} value) : this()", PascalName, field.GetStorageType(code)))
                        code.WriteLine("this.Value = value;");

                    field.WriteMember(code);
                    WriteMembers(code, new[] {field});
                }
                using (code.CodeRegion("Operators and Comparisons"))
                {
                    using (code.WriteBlock("public override string ToString()"))
                        code.WriteLine("return Value.ToString();");
                    using (code.WriteBlock("public override int GetHashCode()"))
                        code.WriteLine("return Value.GetHashCode();");
                    using (code.WriteBlock("public override bool Equals(object obj)"))
                        code.WriteLine("return obj is {0} ? Equals(({0})obj) : base.Equals(obj);", PascalName);

                    using (code.WriteBlock("public bool Equals({0} other)", PascalName))
                        code.WriteLine("return {0} && other.{0} " +
                                       "? {1}.Equals(other.{1}) : {0} == other.{0};", 
                                       field.HasBackingName, field.FieldBackingName);

                    using (code.WriteBlock("public int CompareTo({0} other)", PascalName))
                        code.WriteLine("return {0} && other.{0} " +
                                       "? {1}.CompareTo(other.{1}) : {0} ? 1 : other.{0} ? -1 : 0;", 
                                       field.HasBackingName, field.FieldBackingName);

                    code.SetClsCompliant(field.IsClsCompliant);
                    using (code.WriteBlock("public static explicit operator {0}({1} value)",
                                        PascalName, field.GetStorageType(code)))
                    {
                        code.WriteLine("return new {0}(value);", PascalName);
                    }
                    code.SetClsCompliant(field.IsClsCompliant);
                    using (code.WriteBlock("public static explicit operator {0}({1} value)",
                        field.GetStorageType(code), PascalName))
                    {
                        code.WriteLine("return value.Value;");
                    }

                    using (code.WriteBlock("public static bool operator ==({0} x, {0} y)", PascalName))
                        code.WriteLine("return x.Equals(y);");
                    using (code.WriteBlock("public static bool operator !=({0} x, {0} y)", PascalName))
                        code.WriteLine("return !x.Equals(y);");
                }
            }
        }
    }
}
