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

                string code = File.ReadAllText(srcfile);
                sources.Add(code);
            }
            sourceFiles = sources.ToArray();
        }

        void Compile(IEnumerable<string> sourceFiles, Action<string> error)
        {
            string temp = Path.GetTempFileName();
            try
            {
                List<string> sources = new List<string>();
                sources.AddRange(sourceFiles);
                foreach(string res in GetType().Assembly.GetManifestResourceNames())
                {
                    if (res.StartsWith(GetType().Namespace + ".Library."))
                        sources.Add(new StreamReader(
                                        GetType().Assembly.GetManifestResourceStream(res) ?? Stream.Null
                                        ).ReadToEnd());
                }

                CodeDomProvider csc = new CSharpCodeProvider();
                CompilerParameters args = new CompilerParameters();
                args.GenerateExecutable = false;
                args.IncludeDebugInformation = false;
                args.ReferencedAssemblies.Add("System.dll");
                args.ReferencedAssemblies.Add("System.Data.dll");
                args.ReferencedAssemblies.Add("System.Xml.dll");
                args.OutputAssembly = temp;
                CompilerResults results = csc.CompileAssemblyFromSource(args, sources.ToArray());

                foreach (CompilerError ce in results.Errors)
                {
                    error(String.Format("{0}({1},{2}: {5} {3}: {4}", 
                        ce.FileName, ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText,
                        ce.IsWarning ? "warning" : "error"));
                }

                if (!File.Exists(temp))
                    throw new FileNotFoundException(new FileNotFoundException().Message, temp);
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}
