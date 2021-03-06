﻿#region Copyright (c) 2012 Roger O Knapp
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Types
{
    class ServiceTypeGenerator : BaseTypeGenerator<ServiceInfo>
    {
        private Dictionary<string, ComplexType> _types;

        public ServiceTypeGenerator(ServiceInfo type)
            : base(type)
        {
            _types = new Dictionary<string, ComplexType>(StringComparer.Ordinal);
        }

        ComplexType FindType(string name)
        {
            ComplexType t;
            if (_types.TryGetValue(name, out t))
                return t;
            t = Type.ParentConfig.ResolveName<ComplexType>(Type, name);
            _types.Add(name, t);
            return t;
        }

        public override void DeclareType(CsCodeWriter code)
        {
            WriteInterface(code);
            WriteClientStub(code);
        }

        public void WriteInterface(CsCodeWriter code)
        {
            using(code.DeclareInterface(new CodeItem("I" + PascalName) { Access = Access, }, new string[0]))
            {
                foreach(ServiceMethod m in Type.Methods)
                {
                    ComplexType response = String.IsNullOrEmpty(m.Response) ? null : FindType(m.Response);
                    ComplexType request = String.IsNullOrEmpty(m.Request) ? null : FindType(m.Request);

                    code.WriteLine("{0} {1}({2});",
                                   response == null ? "void" : (CsCodeWriter.Global + response.QualifiedName),
                                   CodeWriter.ToPascalCase(m.Name),
                                   request == null ? "" : (CsCodeWriter.Global + request.QualifiedName + " " + CodeWriter.ToCamelCase(request.Name))
                        );
                }
            }
        }

        public void WriteClientStub(CsCodeWriter code)
        {
            string[] derives = new string[]
                                   {
                                       String.Format("I{0}", PascalName),
                                       String.Format("{0}NClassify.Library.IDispatchStub", CsCodeWriter.Global)
                                   };
            using (code.DeclareClass(
                new CodeItem(PascalName)
                    {
                        Access = Access,
                        XmlName = Name,
                    },
                    derives))
            {
                code.WriteLine("private bool _dispose;");
                code.WriteLine("private readonly {0}NClassify.Library.IDispatchStub _dispatch;", CsCodeWriter.Global);

                code.WriteLine("public {1}({0}NClassify.Library.IDispatchStub dispatch)", CsCodeWriter.Global, PascalName);
                code.WriteLineIndent(": this(dispatch, true) { }");

                using (code.WriteBlock("public {1}({0}NClassify.Library.IDispatchStub dispatch, bool dispose)", CsCodeWriter.Global, PascalName))
                {
                    code.WriteLine("if (dispatch == null) throw new {0}System.ArgumentNullException(\"dispatch\");", CsCodeWriter.Global);
                    code.WriteLine("_dispatch = dispatch;");
                    code.WriteLine("_dispose = dispose;");
                }

                using (code.WriteBlock("public void Dispose()"))
                {
                    using (code.WriteBlock("if (_dispose)"))
                    {
                        code.WriteLine("_dispose = false;");
                        code.WriteLine("_dispatch.Dispose();");
                    }
                }

                using (code.WriteBlock("protected virtual void Invoke<TRequest, TResponse>(string methodName, TRequest request, TResponse response) "
                                       + "where TRequest : class, {0}NClassify.Library.IMessage where TResponse : class, {0}NClassify.Library.IBuilder"
                                       , CsCodeWriter.Global))
                {
                    //code.WriteLine("if (_dispose) throw new {0}System.ObjectDisposedException(GetType().FullName);", CsCodeWriter.Global);
                    code.WriteLine("_dispatch.CallMethod(methodName, request, response);");
                }

                using (code.WriteBlock("void {0}NClassify.Library.IDispatchStub.CallMethod<TRequest, TResponse>(string methodName, TRequest request, TResponse response) "
                    //+ "where TRequest : class, {0}NClassify.Library.IMessage where TResponse : class, {0}NClassify.Library.IBuilder"
                                       , CsCodeWriter.Global))
                {
                    code.WriteLine("Invoke(methodName, request, response);");
                }

                foreach (ServiceMethod m in Type.Methods)
                {
                    ComplexType response = String.IsNullOrEmpty(m.Response) ? null : FindType(m.Response);
                    ComplexType request = String.IsNullOrEmpty(m.Request) ? null : FindType(m.Request);

                    using (code.WriteBlock("public {0} {1}({2})",
                                   response == null ? "void" : (CsCodeWriter.Global + response.QualifiedName),
                                   CodeWriter.ToPascalCase(m.Name),
                                   request == null ? "" : (CsCodeWriter.Global + request.QualifiedName + " " + CodeWriter.ToCamelCase(request.Name))
                        ))
                    {
                        if (response != null)
                        {
                            code.WriteLine("{0}{1} response = new {0}{1}();", CsCodeWriter.Global, response.ImplementationName);
                            code.WriteLine("Invoke(\"{0}\", {1}, response);", m.Name,
                                request == null ? (CsCodeWriter.Global + "NClassify.Library.EmptyMessage.DefaultInstance") : CodeWriter.ToCamelCase(request.Name));
                            code.WriteLine("return response;");
                        }
                        else
                        {
                            code.WriteLine("Invoke(\"{0}\", {1}, {2});", m.Name,
                                request == null ? (CsCodeWriter.Global + "NClassify.Library.EmptyMessage.DefaultInstance") : CodeWriter.ToCamelCase(request.Name),
                                CsCodeWriter.Global + "NClassify.Library.EmptyMessage.DefaultInstance");
                        }
                    }
                }

                WriteDispatchStub(code);
                WriteServerStub(code);
            }
        }

        public void WriteDispatchStub(CsCodeWriter code)
        {
            string[] derives = new string[]
                                   {
                                       String.Format("{0}NClassify.Library.IDispatchStub", CsCodeWriter.Global)
                                   };
            using (code.DeclareClass(
                new CodeItem("Dispatch") { Access =  FieldAccess.Public }, derives))
            {
                code.WriteLine("private bool _dispose;");
                code.WriteLine("private readonly I{0} _dispatch;", PascalName);

                code.WriteLine("public Dispatch(I{0} dispatch)", PascalName);
                code.WriteLineIndent(": this(dispatch, true) { }");

                using (code.WriteBlock("public Dispatch(I{0} dispatch, bool dispose)", PascalName))
                {
                    code.WriteLine("if (dispatch == null) throw new {0}System.ArgumentNullException(\"dispatch\");", CsCodeWriter.Global);
                    code.WriteLine("_dispatch = dispatch;");
                    code.WriteLine("_dispose = dispose && dispatch is {0}System.IDisposable;", CsCodeWriter.Global);
                }

                using (code.WriteBlock("public void Dispose()"))
                {
                    using (code.WriteBlock("if (_dispose)"))
                    {
                        code.WriteLine("_dispose = false;");
                        code.WriteLine("(({0}System.IDisposable)_dispatch).Dispose();", CsCodeWriter.Global);
                    }
                }

                using (code.WriteBlock("public void CallMethod<TRequest, TResponse>(string methodName, TRequest request, TResponse response) "
                                       + "where TRequest : class, {0}NClassify.Library.IMessage where TResponse : class, {0}NClassify.Library.IBuilder"
                                       , CsCodeWriter.Global))
                {
                    string[] names = Type.Methods.Select(m => m.Name).ToArray();
                    Array.Sort(names, StringComparer.Ordinal);
                    var methods = Type.Methods.ToLookup(m => m.Name);

                    code.WriteLine(
                        "int ordinal = {0}System.Array.BinarySearch(new string[] {{ \"{1}\" }}, methodName, global::System.StringComparer.Ordinal);",
                        CsCodeWriter.Global, String.Join("\", \"", names));
                
                    int ordinal = 0;
                    using (code.WriteBlock("switch(ordinal)"))
                    {
                        foreach (string name in names)
                        {
                            ServiceMethod m = methods[name].First();
                            ComplexType response = String.IsNullOrEmpty(m.Response) ? null : FindType(m.Response);
                            ComplexType request = String.IsNullOrEmpty(m.Request) ? null : FindType(m.Request);
                            using(code.WriteBlock("case {0}:", ordinal++))
                            {
                                string call = String.Format("_dispatch.{0}({1})", CodeWriter.ToPascalCase(m.Name),
                                    request == null ? "" : ("(" + CsCodeWriter.Global + request.QualifiedName + ")(object)request"));
                                if (response != null)
                                    call = String.Format("response.MergeFrom({0})", call);
                                code.WriteLine("{0};", call);
                                code.WriteLine("break;");
                            }
                        }
                        using(code.WriteBlock("default:"))
                            code.WriteLine("throw new {0}System.MissingMethodException(typeof(I{1}).FullName, methodName);", CsCodeWriter.Global, PascalName);
                    }
                }
            }
        }

        public void WriteServerStub(CsCodeWriter code)
        {
            string[] derives = new string[]
                                   {
                                       String.Format("{0}NClassify.Library.IServerStub", CsCodeWriter.Global)
                                   };
            using (code.DeclareClass(
                new CodeItem("ServerStub") { Access = FieldAccess.Public }, derives))
            {
                code.WriteLine("private bool _dispose;");
                code.WriteLine("private readonly {0}NClassify.Library.IDispatchStub _dispatch;", CsCodeWriter.Global);

                code.WriteLine("public ServerStub(I{0} dispatch)", PascalName);
                code.WriteLineIndent(": this(new Dispatch(dispatch), true) { }");

                code.WriteLine("public ServerStub(I{0} dispatch, bool dispose)", PascalName);
                code.WriteLineIndent(": this(new Dispatch(dispatch), dispose) { }");

                code.WriteLine("public ServerStub({0}NClassify.Library.IDispatchStub dispatch)", CsCodeWriter.Global);
                code.WriteLineIndent(": this(dispatch, true) { }");

                using (code.WriteBlock("public ServerStub({0}NClassify.Library.IDispatchStub dispatch, bool dispose)", CsCodeWriter.Global))
                {
                    code.WriteLine("if (dispatch == null) throw new {0}System.ArgumentNullException(\"dispatch\");", CsCodeWriter.Global);
                    code.WriteLine("_dispatch = dispatch;");
                    code.WriteLine("_dispose = dispose;");
                }

                using (code.WriteBlock("public void Dispose()"))
                {
                    using (code.WriteBlock("if (_dispose)"))
                    {
                        code.WriteLine("_dispose = false;");
                        code.WriteLine("_dispatch.Dispose();");
                    }
                }


                using (code.WriteBlock("public {0}NClassify.Library.IMessage CallMethod(string methodName, {0}System.Action<{0}NClassify.Library.IBuilder> readInput)"
                                       , CsCodeWriter.Global))
                {
                    string[] names = Type.Methods.Select(m => m.Name).ToArray();
                    Array.Sort(names, StringComparer.Ordinal);
                    var methods = Type.Methods.ToLookup(m => m.Name);

                    code.WriteLine(
                        "int ordinal = {0}System.Array.BinarySearch(new string[] {{ \"{1}\" }}, methodName, global::System.StringComparer.Ordinal);",
                        CsCodeWriter.Global, String.Join("\", \"", names));

                    int ordinal = 0;
                    using (code.WriteBlock("switch(ordinal)"))
                    {
                        foreach (string name in names)
                        {
                            ServiceMethod m = methods[name].First();
                            ComplexType response = String.IsNullOrEmpty(m.Response) ? null : FindType(m.Response);
                            ComplexType request = String.IsNullOrEmpty(m.Request) ? null : FindType(m.Request);
                            using (code.WriteBlock("case {0}:", ordinal++))
                            {
                                if (request != null)
                                {
                                    code.WriteLine("{0}{1} request = new {0}{1}();", CsCodeWriter.Global, request.ImplementationName);
                                    code.WriteLine("readInput(request);");
                                }
                                if (response != null)
                                {
                                    code.WriteLine("{0}{1} response = new {0}{1}();", CsCodeWriter.Global, response.ImplementationName);
                                }
                                code.WriteLine("_dispatch.CallMethod(methodName, {0}, {1});",
                                    request == null ? (CsCodeWriter.Global + "NClassify.Library.EmptyMessage.DefaultInstance") : "request",
                                    response == null ? (CsCodeWriter.Global + "NClassify.Library.EmptyMessage.DefaultInstance") : "response"
                                    );
                                code.WriteLine("return {0};",
                                               response == null
                                                   ? (CsCodeWriter.Global + "NClassify.Library.EmptyMessage.DefaultInstance")
                                                   : "response");
                            }
                        }
                        using (code.WriteBlock("default:"))
                            code.WriteLine("throw new {0}System.MissingMethodException(typeof(I{1}).FullName, methodName);", CsCodeWriter.Global, PascalName);
                    }
                }
            }
        }
    }
}
