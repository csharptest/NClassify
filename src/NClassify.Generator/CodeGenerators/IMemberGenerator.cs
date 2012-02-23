using System;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators
{
    internal interface IMemberGenerator<T> where T : CodeWriter
    {
        void DeclareTypes(T code);
        void DeclareStaticData(T code);
        void WriteMember(T code);
    }
}
