using System.Collections.Generic;
using System.Linq;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections
{
    public sealed class WriteOnlyPropertyInspection : InspectionBase
    {
        public WriteOnlyPropertyInspection(RubberduckParserState state)
            : base(state, CodeInspectionSeverity.Suggestion)
        {
        }

        public override string Meta { get { return InspectionsUI.WriteOnlyPropertyInspectionMeta; } }
        public override string Description { get { return InspectionsUI.WriteOnlyPropertyInspectionResultFormat; } }
        public override CodeInspectionType InspectionType { get { return CodeInspectionType.CodeQualityIssues; } }

        public override IEnumerable<CodeInspectionResultBase> GetInspectionResults()
        {
            var declarations = UserDeclarations.ToList();
            var setters = declarations
                .Where(item => 
                       (item.Accessibility == Accessibility.Implicit || 
                        item.Accessibility == Accessibility.Public || 
                        item.Accessibility == Accessibility.Global)
                    && (item.DeclarationType == DeclarationType.PropertyLet ||
                        item.DeclarationType == DeclarationType.PropertySet)
                    && !declarations.Where(declaration => declaration.IdentifierName == item.IdentifierName)
                        .Any(accessor => !accessor.IsBuiltIn && accessor.DeclarationType == DeclarationType.PropertyGet));

            //note: if property has both Set and Let accessors, this generates 2 results.
            return setters.Select(setter =>
                new WriteOnlyPropertyInspectionResult(this, string.Format(Description, setter.IdentifierName), setter));
        }
    }

    public class WriteOnlyPropertyInspectionResult : CodeInspectionResultBase
    {
        public WriteOnlyPropertyInspectionResult(IInspection inspection, string result, Declaration target) 
            : base(inspection, result, target)
        {
        }

        // todo: override quickfixes
        //public override IEnumerable<CodeInspectionQuickFix> QuickFixes { get; private set; }
    }
}