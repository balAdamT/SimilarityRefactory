using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Algorithm
{
    public class MethodFragmentsInCompilation : ISyntaxSource
    {
        private readonly Compilation compilation;
        private readonly int minDepth;

        public MethodFragmentsInCompilation(Compilation compilation, int minDepth = 0)
        {
            this.compilation = compilation;
            this.minDepth = minDepth;
        }

        public IEnumerable<SyntaxNode> Fetch()
        {
            return from tree in compilation.SyntaxTrees
                   from @class in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                   from method in @class.Members.Where<MemberDeclarationSyntax>(member => member.Kind() == SyntaxKind.MethodDeclaration)
                   from node in method.DescendantNodes().Where(node => node.DescendantNodes().Count() >= minDepth)
                   select node;
        }
    }
}
