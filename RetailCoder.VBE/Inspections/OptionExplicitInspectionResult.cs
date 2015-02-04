using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Microsoft.Vbe.Interop;
using Rubberduck.Extensions;
using Rubberduck.VBA;

namespace Rubberduck.Inspections
{
    [ComVisible(false)]
    public class OptionExplicitInspectionResult : CodeInspectionResultBase
    {
        public OptionExplicitInspectionResult(string inspection, CodeInspectionSeverity type, QualifiedModuleName qualifiedName) 
            : base(inspection, type, qualifiedName, null)
        {
        }

        public override IDictionary<string, Action<VBE>> GetQuickFixes()
        {
            return
                new Dictionary<string, Action<VBE>>
                {
                    {"Specify Option Explicit", SpecifyOptionExplicit}
                };
        }

        private void SpecifyOptionExplicit(VBE vbe)
        {
            throw new NotImplementedException();
        }
    }
}