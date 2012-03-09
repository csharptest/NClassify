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
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class IsValidConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;

        public IsValidConstraintGenerator(BaseFieldGenerator field)
        {
            _field = field;
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            using (code.WriteBlock("if (!value.IsValid())"))
            {
                code.WriteLine("if (onError == null)");
                code.WriteLineIndent("return false;");
                code.WriteLine(
                    "{0}System.Collections.Generic.List<{0}NClassify.Library.ValidationError> errors = " +
                    "new {0}System.Collections.Generic.List<{0}NClassify.Library.ValidationError>();",
                    CsCodeWriter.Global
                    );
                code.WriteLine("value.GetBrokenRules(errors.Add);");
                code.WriteLine("onError(new {0}NClassify.Library.ValidationError(TypeFields.{1}, errors));",
                    CsCodeWriter.Global, _field.PropertyName);
                code.WriteLine("return false;");
            }
        }
    }
}
