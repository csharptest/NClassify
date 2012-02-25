using System;
using System.IO;
using CSharpTest.Net.Commands;
using NClassify.Generator.CodeGenerators;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.ComponentModel;
using System.Collections.Generic;

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
                CompilerResults results = csc.CompileAssemblyFromSource(args, 
                    code,
                    new StreamReader(
                        GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Library.ByteArray.cs")
                        ).ReadToEnd(),
                    new StreamReader(
                        GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Library.Interfaces.cs")
                        ).ReadToEnd()
                    );

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

        public void Example()
        {
            NClassifyConfig cfg = new NClassifyConfig();
            cfg.Settings.Namespace = "NClassify.Example";
            List<RootItem> root = new List<RootItem>();
            //root.Add(new ImportFile { File=""});
            uint id = 1;
            root.AddRange(
                new BaseType[] 
                {
                new EnumType
                     {
                         Name = "SampleEnum",
                         Values = new []
                                      {
                                          new EnumType.Item { Name = "One", Value = 1 },
                                          new EnumType.Item { Name = "Two", Value = 2 },
                                          new EnumType.Item { Name = "Three", Value = 3 },
                                      }
                     },
                 new SimpleType
                     {
                         Name = "SimpleInt32",
                         Type = FieldType.Int32,
                         Validation = new [] { new RangeConstraint { MinValue = "1" } }
                     },
                 new SimpleType
                     {
                         Name = "SimpleBytes",
                         Type = FieldType.Bytes,
                         Validation = new [] { new LengthConstraint { MinLength = 1, MaxLength = 255 } }
                     },
                 new SimpleType
                     {
                         Name = "SimpleText",
                         Type = FieldType.String,
                         Validation = new []
                                          {
                                              new PredefinedValue
                                                  { Values = new string[] { "abc", "def", "ghi", "jkl", "mno", "pqr", "stu", "vwx", "yz" } }
                                          }
                     },
                 new SimpleType
                     {
                         Name = "SimpleUri",
                         Type = FieldType.String,
                         Validation = new ValidationRule[]
                                          {
                                              new LengthConstraint { MinLength = 8, MaxLength = 2048 },
                                              new MatchConstraint { Pattern = @"^https?\://.+", IgnoreCase = true },
                                              new CodedConstraint
                                                  {
                                                      Language = GeneratorLanguage.CSharp,
                                                      Code = "IsValidUri(value)",
                                                      MethodBody = "global::System.Uri tmp;\r\n" +
                                                                   "return global::System.Uri.TryCreate(value, global::System.UriKind.Absolute, out tmp);",
                                                  }
                                          }
                     },
                 new ComplexType
                     {
                         Name = "ChildMessage",
                         Fields = new FieldInfo[]
                         {
                            new SimpleTypeRef
                            {
                                FieldId = id++,
                                
                            }
                         }
                     },
                 new ComplexType
                     {
                         Name = "AllTypesTest",
                         Fields = AllFields()
                     }
                }
            );
        }

        private static FieldInfo[] AllFields()
        {
            return null;   
        }
    }
}
