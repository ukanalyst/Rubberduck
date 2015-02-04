﻿using System;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Rubberduck.Inspections;
using Rubberduck.VBA;

namespace Rubberduck.UI.CodeInspections
{
    [ComVisible(false)]
    public class NavigateCodeIssueEventArgs : EventArgs
    {
        public NavigateCodeIssueEventArgs(QualifiedModuleName qualifiedName, ParserRuleContext context)
        {
            _qualifiedName = qualifiedName;
            _context = context;
        }

        private readonly QualifiedModuleName _qualifiedName;
        public QualifiedModuleName QualifiedName { get { return _qualifiedName; } }

        private readonly ParserRuleContext _context;
        public ParserRuleContext Context { get { return _context; } }
    }
}