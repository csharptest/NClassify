using System;
using System.Collections.Generic;

namespace NClassify.Generator.CodeGenerators.Fields
{
    class FieldFactory
    {
        private delegate BaseFieldGenerator FactoryMethod(FieldInfo field);

        private static readonly Dictionary<FieldType, FactoryMethod> Fields =
            new Dictionary<FieldType, FactoryMethod>()
                {
                    {FieldType.Complex, f => new ComplexFieldGenerator(f)},
                    {FieldType.Enum, f => new EnumFieldGenerator(f)},
                    {FieldType.Simple, f => new SimpleFieldGenerator(f)},
                    {FieldType.Boolean, f => new BooleanFieldGenerator(f)},
                    {FieldType.Bytes, f => new BytesFieldGenerator(f)},
                    {FieldType.Int8, f => new Int8FieldGenerator(f)},
                    {FieldType.UInt8, f => new UInt8FieldGenerator(f)},
                    {FieldType.Int16, f => new Int16FieldGenerator(f)},
                    {FieldType.UInt16, f => new UInt16FieldGenerator(f)},
                    {FieldType.Int32, f => new Int32FieldGenerator(f)},
                    {FieldType.UInt32, f => new UInt32FieldGenerator(f)},
                    {FieldType.Int64, f => new Int64FieldGenerator(f)},
                    {FieldType.UInt64, f => new UInt64FieldGenerator(f)},
                    {FieldType.Float, f => new FloatFieldGenerator(f)},
                    {FieldType.Double, f => new DoubleFieldGenerator(f)},
                    {FieldType.Guid, f => new GuidFieldGenerator(f)},
                    {FieldType.DateTime, f => new DateTimeFieldGenerator(f)},
                    {FieldType.TimeSpan, f => new TimeSpanFieldGenerator(f)},
                    {FieldType.String, f => new StringFieldGenerator(f)},
                    //{FieldType.Uri, f => new UriFieldGenerator(f)},
                };

        public static BaseFieldGenerator Create(FieldInfo field)
        {
            FactoryMethod factory;
            if (!Fields.TryGetValue(field.Type, out factory))
                throw new ApplicationException("Unknown field type " + field.Type);

            if (field.IsArray)
                return new ArrayFieldGenerator(field, factory(field));

            return factory(field);
        }
    }
}
