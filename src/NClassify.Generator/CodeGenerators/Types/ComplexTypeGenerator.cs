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
        public override void WriteMember(CsCodeWriter code)
        {
            string[] derives = new string[0];
            using (code.DeclareClass(
                new CodeItem(PascalName)
                    {
                        Access = Access,
                        XmlName = Name,
                    }, 
                    derives))
            {
                Fields.ForAll(x => x.DeclareStaticData(code));
                Fields.ForAll(x => x.DeclareInstanceData(code));
                Fields.ForAll(x => x.WriteMember(code));
            }
        }
    }
}
