using System;
using System.IO;
using CSharpTest.Net.Commands;
using NClassify.Generator.CodeGenerators;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.ComponentModel;

namespace NClassify.Generator
{
    class Program
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

        public void Generate(string inputXml)
        {
            NClassifyConfig config = NClassifyConfig.Read(inputXml, null);

            CsCodeGenerator gen = new CsCodeGenerator(config);
            string srcfile = Path.ChangeExtension(config.FilePath, ".cs");
            using(TextWriter wtr = File.CreateText(srcfile))
                gen.GenerateCode(wtr);

            string code = File.ReadAllText(srcfile);
            Console.WriteLine(code);

            int errors = 0;
            Compile(code, e => { Console.Error.WriteLine(e); errors++; });
            //if (errors > 0)
            //    throw new ApplicationException(errors + " error(s) compiling code.");
        }

        private void Compile(string code, Action<string> error)
        {
            string temp = Path.GetTempFileName();
            try
            {
                CodeDomProvider csc = new CSharpCodeProvider();
                CompilerParameters args = new CompilerParameters();
                args.GenerateExecutable = false;
                args.IncludeDebugInformation = false;
                args.ReferencedAssemblies.Add("System.dll");
                args.ReferencedAssemblies.Add("System.Data.dll");
                args.ReferencedAssemblies.Add("System.Xml.dll");
                args.OutputAssembly = temp;
                CompilerResults results = csc.CompileAssemblyFromSource(args, code);

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
