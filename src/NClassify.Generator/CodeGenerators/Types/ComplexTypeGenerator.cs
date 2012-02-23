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

        public override void DeclareType(CsCodeWriter code)
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
                WriteChildren(code, Type.ChildTypes);
                Fields.ForAll(x => x.DeclareTypes(code));

                using (code.CodeRegion("Static Data"))
                    Fields.ForAll(x => x.DeclareStaticData(code));
                using (code.CodeRegion("Instance Fields"))
                    Fields.ForAll(x => x.DeclareInstanceData(code));
                using (code.CodeRegion("Instance Members"))
                    Fields.ForAll(x => x.WriteMember(code));
            }
        }
    }
}
