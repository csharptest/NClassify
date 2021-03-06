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
using System.IO;
using NClassify.Generator.CodeWriters;
using NClassify.Generator.CodeGenerators.Types;

namespace NClassify.Generator.CodeGenerators
{
    public abstract class BaseGenerator<TWriter> where TWriter : CodeWriter
    {
        private readonly NClassifyConfig _config;

        protected BaseGenerator(NClassifyConfig config)
        {
            _config = config;
        }

        public NClassifyConfig Config { get { return _config; } }

        public virtual void GenerateCode(TextWriter writer)
        {
            using (TWriter code = OpenWriter(writer))
            {
                using (code.WriteNamespace(Config.RootNamespace.QualifiedName.Split('.', ':')))
                {
                    GenerateChildren(code, Config.RootNamespace);
                }
                CloseWriter(code);
            }
        }

        protected virtual void GenerateChildren(TWriter code, BaseType parent)
        {
            List<BaseTypeGenerator> types = new List<BaseTypeGenerator>();
            Config.GetEnumerations(parent).ForAll(x => types.Add(new EnumTypeGenerator(x)));
            Config.GetSimpleTypes(parent).ForAll(x => types.Add(new SimpleTypeGenerator(x)));
            Config.GetComplexTypes(parent).ForAll(x => types.Add(new ComplexTypeGenerator(x)));
            Config.GetServices(parent).ForAll(x => types.Add(new ServiceTypeGenerator(x)));

            types.OfType<IMemberGenerator<TWriter>>()
                .ForAll(x => x.DeclareStaticData(code));

            types.OfType<IMemberGenerator<TWriter>>()
                .ForAll(x => x.DeclareTypes(code));
        }

        protected abstract TWriter OpenWriter(TextWriter writer);
        protected virtual void CloseWriter(TWriter cw) { cw.Dispose(); }
    }
}
