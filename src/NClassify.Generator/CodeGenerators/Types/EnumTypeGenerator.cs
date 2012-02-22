using System;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    class EnumTypeGenerator : BaseTypeGenerator<EnumType>
    {
        public EnumTypeGenerator(EnumType type)
            : base(type)
        { }

        public override void WriteMember(CsCodeWriter code)
        {
            using (code.DeclareEnum(
                new CodeItem(PascalName)
                    {
                        Access = Access,
                        XmlName = Name,
                    }))
            {
                foreach (var item in Type.Values)
                    code.WriteEnumValue(item.Name, item.Value);
            }
        }
    }
}
