using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.Data;
using SimilarityAnalyzer.Logic;
using SimilarityAnalyzer.SimilarityFinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.SimilarityFinders
{
    public class CommonSubTreeFinder: Finder
    {
        public CommonSubTreeFinder(ClassDeclarationSyntax @class) : base(@class)
        {
        }

        protected override void FindSimilarities()
        {
            var classMap = new MethodFragmentMap();

            foreach (MethodDeclarationSyntax method
                in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
            {
                TreeExplorer.ForEachInnerNode(method, fragment => classMap.AddFragmentOfMethod(method, fragment));
            }

            var outs = classMap.GetOuterPairs();
            var ins = classMap.GetInnerPairs();
            var pairs = outs.Concat(ins);

            List<NodePair> similarities = new List<NodePair>();
            foreach (var pair in pairs)
            {
                if (pair.Left.IsEquivalentTo(pair.Right))
                {
                    similarities.Add(pair);
                }
            }

            MatchData.InnerPairs = ins.Count();
            MatchData.OuterPairs = outs.Count();
            MatchData.Matches = Similarities.Count();
            MatchData.MatchNodeLengths = Similarities.Select(pair => pair.Count());
        }
    }
}
