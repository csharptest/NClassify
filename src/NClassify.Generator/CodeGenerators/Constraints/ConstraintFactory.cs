using System;
using NClassify.Generator.CodeGenerators.Fields;

namespace NClassify.Generator.CodeGenerators.Constraints
{
    class ConstraintFactory
    {
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
            
            throw new ApplicationException("Invalid constraint type " + rule.GetType());
        }
    }
}
