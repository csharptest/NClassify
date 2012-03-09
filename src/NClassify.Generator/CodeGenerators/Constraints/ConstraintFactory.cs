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
using System.Collections.Generic;
using System.Linq;
using NClassify.Generator.CodeGenerators.Fields;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class ConstraintFactory
    {
        public static IEnumerable<BaseConstraintGenerator> Create(BaseFieldGenerator field, IEnumerable<ValidationRule> rules)
        {
            return rules.SafeEnum().Select(x => Create(field, x));
        }

        public static BaseConstraintGenerator Create(BaseFieldGenerator field, ValidationRule rule)
        {
            if (rule is LengthConstraint)
                return new LengthConstraintGenerator(field, (LengthConstraint)rule);
            if (rule is RangeConstraint)
                return new RangeConstraintGenerator(field, (RangeConstraint)rule);
            if (rule is MatchConstraint)
                return new MatchConstraintGenerator(field, (MatchConstraint)rule);
            if (rule is PredefinedValue)
                return new ListConstraintGenerator(field, (PredefinedValue)rule);
            if (rule is CodedConstraint)
                return new CodeConstraintGenerator(field, (CodedConstraint) rule);
            
            throw new ApplicationException("Invalid constraint type " + rule.GetType());
        }
    }
}
