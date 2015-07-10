using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Xbim.Gppg;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Xbim.ExpressParser
{
    internal partial class Parser
    {
        readonly Scanner _scanner;

        internal Parser(Scanner lex): base(lex)
        {
            _scanner = lex;
        }
    }
}
