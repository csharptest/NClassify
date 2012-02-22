using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators
{
    internal interface IMemberGenerator<T> where T : CodeWriter
    {
        void DeclareStaticData(T code);
        void DeclareInstanceData(T code);
        void WriteMember(T code);
    }
}
