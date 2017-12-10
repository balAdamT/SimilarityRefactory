﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.Logic;
using SyntaxComparision.Interfaces;

namespace SyntaxComparision.Algorithm
{
  public class LeavesInClass : ISyntaxSource
  {
    private readonly ClassDeclarationSyntax @class;

    public LeavesInClass(ClassDeclarationSyntax @class)
    {
      this.@class = @class;
    }

    public IEnumerable<SyntaxNode> Fetch()
    {
      foreach (MethodDeclarationSyntax method
        in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
        foreach (var node in method.DescendantNodes().Where(node => node.ChildNodes().Any() == false))
          yield return node;
    }
  }
}