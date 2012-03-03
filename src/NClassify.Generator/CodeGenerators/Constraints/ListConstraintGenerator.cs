using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class ListConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly PredefinedValue _rule;

        public ListConstraintGenerator(BaseFieldGenerator field, PredefinedValue rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void DeclareStaticData(CsCodeWriter code)
        {
            code.Write("private static readonly {0}[] __in_{1} = new {0}[] {{", _field.GetStorageType(code), _field.CamelName);
            int ix = 0;
            string[] list = (string[]) _rule.Values.Clone();
            Array.Sort(list);

            foreach (string possible in _rule.Values)
            {
                if (ix++ > 0) code.Write(", ");
                code.Write(code.MakeConstant(_field.FieldType, possible));
            }
            code.WriteLine("};");
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            if (_field.FieldType != FieldType.String)
                throw new ApplicationException("The possible values constraints only applies to fields of type String.");

            using (code.WriteBlock("if (global::System.Array.BinarySearch(__in_{0}, value) < 0)", _field.CamelName))
            {
                code.WriteLine(
                    "if (onError != null) onError(new {0}NClassify.Library.ValidationError(TypeFields.{2}, " +
                    "{0}NClassify.Library.Resources.MustBeOneOf, TypeFields.{2}, string.Join(\", \", __in_{1})));",
                    CsCodeWriter.Global, _field.CamelName, _field.PropertyName);
                code.WriteLine("return false;");
            }
        }
    }
}
