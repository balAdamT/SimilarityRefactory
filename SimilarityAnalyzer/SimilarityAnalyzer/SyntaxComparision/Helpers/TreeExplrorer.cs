﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Logic
{
    static class TreeExplorer
    {
        public static void ForEachNode(this SyntaxNode parent, Action<SyntaxNode> action)
        {
            action.Invoke(parent);

            foreach(var child in parent.DescendantNodes())
            {
                action.Invoke(parent);
            }
        }

        public static void ForEachLeaf(this SyntaxNode parent, Action<SyntaxNode> action)
        {
            foreach(var node in parent.DescendantNodes().Where(node => node.ChildNodes().Any() == false))
            {
                action.Invoke(node);
            }
        }

        public static void ForEachInnerNode(this SyntaxNode parent, Action<SyntaxNode> action)
        {
            foreach (var node in parent.DescendantNodes().Where(node => node.ChildNodes().Any() == true))
            {
                action.Invoke(node);
            }
        }
    }
}
