using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;

namespace NClassify.Generator.CodeGenerators
{
    static class ReservedWords
    {
        public static bool IsValidFieldName(string name)
        {
            Type type = typeof(NClassify.Library.IBuilder);
            return
                name != "TypeFields" &&
                name != "Initialize" &&
                IsValidFieldName(name, type);
        }

        private static bool IsValidFieldName(string name, Type type)
        {
            return type.GetMembers().FirstOrDefault(m => m.Name == name) == null &&
                type.GetInterfaces().FirstOrDefault(t => !IsValidFieldName(name, t)) == null;
        }
    }
}
