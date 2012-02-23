using System;
using NClassify.Generator.CodeGenerators.Fields;
using NClassify.Generator.CodeWriters;
using System.Text.RegularExpressions;
using System.IO;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class CodeConstraintGenerator : BaseConstraintGenerator
    {
        private readonly BaseFieldGenerator _field;
        private readonly CodedConstraint _rule;

        public CodeConstraintGenerator(BaseFieldGenerator field, CodedConstraint rule)
        {
            _field = field;
            _rule = rule;
        }

        public override void DeclareStaticData(CsCodeWriter code)
        {
            if (code.Language != _rule.Language)
                return;

            Match m;
            if (!String.IsNullOrEmpty(_rule.MethodBody) &&
                (m = new Regex(@"^(?<Name>[a-zA-Z][\w_]*)\(value\)$").Match(_rule.Code)).Success)
            {
                using (code.WriteBlock("private static bool {0}({1} value)", m.Groups["Name"].Value, _field.GetStorageType(code)))
                using (StringReader rdr = new StringReader(_rule.MethodBody))
                {
                    string line;
                    while(null != (line = rdr.ReadLine()))
                    {
                        line = line.Trim();
                        if (line.Length == 0)
                            continue;
                        if (line.StartsWith("}"))
                            code.Indent--;
                        code.WriteLine(line);
                        if (line.EndsWith("{"))
                            code.Indent--;
                    }
                }
            }
        }

        public override void WriteChecks(CsCodeWriter code)
        {
            if (code.Language != _rule.Language)
                return;
            
            code.WriteLine("if (!({0})) return false;", _rule.Code);
        }
    }
}
