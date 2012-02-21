using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators
{
    class CsCodeGenerator : BaseGenerator<CsWriter>
    {
        public CsCodeGenerator(NClassifyConfig config) : base(config)
        {
        }

        protected override CsWriter OpenWriter(System.IO.TextWriter writer)
        {
            CsWriter code = new CsWriter(writer);
            code.WriteFilePreamble();
            return code;
        }

        public override string QualifiedTypeName(FieldType type)
        {
            switch (type)
            {
                case FieldType.None: throw new InvalidOperationException("Unexpected field type: None");
                case FieldType.Enum: throw new InvalidOperationException("Unexpected field type: Enum");
                case FieldType.Value: throw new InvalidOperationException("Unexpected field type: Value");
                case FieldType.Boolean: return "System.Boolean";
                case FieldType.Bytes: return "System.Byte[]";
                case FieldType.Int8: return "System.Byte";
                case FieldType.UInt8: return "System.SByte";
                case FieldType.Int16: return "System.Int16";
                case FieldType.UInt16: return "System.UInt16";
                case FieldType.Int32: return "System.Int32";
                case FieldType.UInt32: return "System.UInt32";
                case FieldType.Int64: return "System.Int64";
                case FieldType.UInt64: return "System.UInt64";
                case FieldType.Float: return "System.Single";
                case FieldType.Double: return "System.Double";
                case FieldType.String: return "System.String";
                case FieldType.Guid: return CsWriter.Global + "System.Guid";
                case FieldType.DateTime: return CsWriter.Global + "System.DateTime";
                case FieldType.TimeSpan: return CsWriter.Global + "System.TimeSpan";
                case FieldType.Uri: return CsWriter.Global + "System.Uri";
                case FieldType.EMail: return CsWriter.Global + "System.Net.Mail.MailAddress";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
