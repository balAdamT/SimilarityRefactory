﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SuperTree
{
    public class LeavesInCompilation : ISyntaxSource
    {
        private readonly Compilation compilation;

        public LeavesInCompilation(Compilation compilation)
        {
            this.compilation = compilation;
        }

        public IEnumerable<SyntaxNode> Fetch()
        {
            return from tree in compilation.SyntaxTrees
                   from @class in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                   from method in @class.Members.Where<MemberDeclarationSyntax>(member => member.Kind() == SyntaxKind.MethodDeclaration)
                   from node in method.DescendantNodes().Where(node => node.ChildNodes().Any() == false)
                   select node;
        }
    }
}
