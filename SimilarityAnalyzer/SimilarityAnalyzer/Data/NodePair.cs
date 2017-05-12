﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Data
{
    public struct NodePair
    {
        public SyntaxNode Left;
        public SyntaxNode Right;

        public NodePair(SyntaxNode left, SyntaxNode right)
        {
            Left = left;
            Right = right;
        }

        public NodePair(Tuple<SyntaxNode, SyntaxNode> pair)
        {
            Left = pair.Item1;
            Right = pair.Item2;
        }

        public Tuple<TextSpan, TextSpan> SpainPair
        {
            get
            {
                return new Tuple<TextSpan, TextSpan>(Left.Span, Right.Span);
            }
        }
    }
}
