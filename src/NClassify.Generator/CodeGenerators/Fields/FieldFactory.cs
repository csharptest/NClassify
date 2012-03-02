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
                    {FieldType.UInt8, f => new UnsignedFieldGenerator(f)},
                    {FieldType.Int16, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt16, f => new UnsignedFieldGenerator(f)},
                    {FieldType.Int32, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt32, f => new UnsignedFieldGenerator(f)},
                    {FieldType.Int64, f => new PrimitiveFieldGenerator(f)},
                    {FieldType.UInt64, f => new UnsignedFieldGenerator(f)},
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
