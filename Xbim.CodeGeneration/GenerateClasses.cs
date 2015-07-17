using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Xbim.CodeGeneration
{
    public class GenerateClasses
    {
        public void Generate()
        {
            var unit = SF.CompilationUnit();
        }
    }
}
