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
using NClassify.Generator.CodeGenerators.Types;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class FieldFactory
    {
        private delegate BaseFieldGenerator FactoryMethod(FieldInfo field);

        private static readonly Dictionary<FieldType, FactoryMethod> Fields =
            new Dictionary<FieldType, FactoryMethod>
                {
                    {FieldType.Complex, f => new ComplexFieldGenerator(f)},
                    {FieldType.Enum, f => new EnumFieldGenerator(f)},
                    {FieldType.Simple, f => new SimpleFieldGenerator(f)},
                    {FieldType.Boolean, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Bytes, f => new BytesFieldGenerator(f)},
                    {FieldType.Int8, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt8, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Int16, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt16, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Int32, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt32, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Int64, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt64, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Float, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Double, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.Guid, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.DateTime, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.TimeSpan, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.String, f => new StringFieldGenerator(f)},
                    //{FieldType.Uri, f => new UriFieldGenerator(f)},
                };

        public static BaseFieldGenerator Create(BaseTypeGenerator type, FieldInfo field)
        {
            field.DeclaringType = type.Type;
            FactoryMethod factory;
            if (!Fields.TryGetValue(field.Type, out factory))
                throw new ApplicationException("Unknown field type " + field.Type);

            if (field.IsArray)
                return new ArrayFieldGenerator(field, factory(field));

            return factory(field);
        }
    }
}
