using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Xbim.CodeGeneration
{
    public class GenerateClasses
    {
        public void Generate()
        {
            var unit = SF.CompilationUnit().AddUsings(
                SF.UsingDirective(SF.IdentifierName("System")),
                SF.UsingDirective(SF.IdentifierName("System.Generic"))
                );
            var ns = SF.NamespaceDeclaration(SF.IdentifierName("Xbim.Ifc"));
            
            unit = unit.AddMembers(ns);
            var cls = SF.ClassDeclaration("IfcWall")
                .AddModifiers( SF.Token( SyntaxKind.PrivateKeyword ) )
                .AddModifiers( SF.Token( SyntaxKind.PartialKeyword ) )
                ;
            ns = ns.AddMembers(cls);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                writer.WriteLine(unit.ToFullString());
            }
            var a = sb.ToString();
        }
    }
}
