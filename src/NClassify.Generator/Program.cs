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
using System.Linq;
using System.IO;
using CSharpTest.Net.Commands;
using NClassify.Generator.CodeGenerators;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.ComponentModel;

namespace NClassify.Generator
{
    partial class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandInterpreter ci = new CommandInterpreter(DefaultCommands.Help, new Program());
                ci.Run(args);
            }
            catch(ApplicationException ae)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine(ae.Message);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine(e.ToString());
            }
            finally
            {
                if(System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press [Enter] to quit...");
                    Console.ReadLine();
                }
            }
        }

        public void Generate(string[] file, [DefaultValue(null)] string nameSpace, [DefaultValue(".cs")] string extension)
        {
            string[] ignore;
            GenerateSource(file, nameSpace, extension, out ignore);
        }

        public void TestGenerate(string[] file, [DefaultValue(null)] string nameSpace, [DefaultValue(".cs")] string extension)
        {
            string[] sourceFiles;
            GenerateSource(file, nameSpace, extension, out sourceFiles);

            int errors = 0;
            Compile(sourceFiles, e => { Console.Error.WriteLine(e); errors++; });

            if (errors > 0)
                Environment.ExitCode = 0xce;
        }

        void GenerateSource(IEnumerable<string> files, string nameSpace, string extension, out string[] sourceFiles)
        {
            List<string> sources = new List<string>();
            foreach (string inputXml in files)
            {
                NClassifyConfig config = NClassifyConfig.Read(inputXml, nameSpace);

                CsCodeGenerator gen = new CsCodeGenerator(config);
                string srcfile = Path.ChangeExtension(config.FilePath, extension ?? ".cs");
                using (TextWriter wtr = File.CreateText(srcfile))
                    gen.GenerateCode(wtr);

                sources.Add(srcfile);
            }
            sourceFiles = sources.ToArray();
        }

        void Compile(IEnumerable<string> sources, Action<string> error)
        {
            CodeDomProvider csc = new CSharpCodeProvider();
            CompilerParameters args = new CompilerParameters();
            args.GenerateInMemory = true;
            args.GenerateExecutable = false;
            args.IncludeDebugInformation = false;
            args.ReferencedAssemblies.Add("System.dll");
            args.ReferencedAssemblies.Add("System.Data.dll");
            args.ReferencedAssemblies.Add("System.Xml.dll");
            args.ReferencedAssemblies.Add(typeof(NClassify.Library.IMessage).Assembly.Location);
            CompilerResults results = csc.CompileAssemblyFromFile(args, sources.ToArray());

            foreach (CompilerError ce in results.Errors)
            {
                error(String.Format("{0}({1},{2}): {5} {3}: {4}", 
                    ce.FileName, ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText,
                    ce.IsWarning ? "warning" : "error"));
            }
        }
    }
}
