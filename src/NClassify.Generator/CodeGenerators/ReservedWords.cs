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
        private static readonly  System.Reflection.Assembly CompiledAssembly;

        static ReservedWords()
        {
            string prefix = typeof(Program).Namespace + ".Library.";
            List<string> sources = new List<string>(
                typeof (Program).Assembly.GetManifestResourceNames()
                    .Where(r => r.EndsWith(".cs") && r.StartsWith(prefix))
                    .Select(r => new StreamReader(typeof (Program).Assembly.GetManifestResourceStream(r)).ReadToEnd())
                );

            CodeDomProvider csc = new CSharpCodeProvider();
            CompilerParameters args = new CompilerParameters();
            args.GenerateInMemory = true;
            args.GenerateExecutable = false;
            args.IncludeDebugInformation = false;
            args.ReferencedAssemblies.Add("System.dll");
            args.ReferencedAssemblies.Add("System.Data.dll");
            args.ReferencedAssemblies.Add("System.Xml.dll");
            CompilerResults results = csc.CompileAssemblyFromSource(args, sources.ToArray());

            foreach (CompilerError ce in results.Errors)
            {
                throw new ApplicationException(
                    String.Format("{0}({1},{2}): {5} {3}: {4}",
                                  ce.FileName, ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText,
                                  ce.IsWarning ? "warning" : "error"));
            }
            CompiledAssembly = results.CompiledAssembly;
        }

        public static bool IsValidFieldName(string name)
        {
            Type type = CompiledAssembly.GetType("NClassify.Library.IMessage", true);
            return 
                name != "TypeFields" &&
                IsValidFieldName(name, type);
        }

        private static bool IsValidFieldName(string name, Type type)
        {
            return type.GetMembers().FirstOrDefault(m => m.Name == name) == null &&
                type.GetInterfaces().FirstOrDefault(t => !IsValidFieldName(name, t)) == null;
        }
    }
}
