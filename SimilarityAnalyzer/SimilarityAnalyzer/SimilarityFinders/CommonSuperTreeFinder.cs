using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.Data;
using Microsoft.CodeAnalysis.CSharp;
using SimilarityAnalyzer.SimilarityFinders;
using SimilarityAnalyzer.Logic;

namespace SimilarityAnalyzer.SimilarityFinders
{
    public class CommonSuperTreeFinder : Finder
    {
        public CommonSuperTreeFinder(ClassDeclarationSyntax @class) : base(@class)
        {
        }

        protected override void FindSimilarities()
        {
            var classMap = new MethodFragmentMap();

            foreach (MethodDeclarationSyntax method
                in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
            {
                TreeExplorer.ForEachLeaf(method, fragment => classMap.AddFragmentOfMethod(method, fragment));
            }

            var outs = classMap.GetOuterPairs();
            var ins = classMap.GetInnerPairs();
            var pairs = outs.Concat(ins);

            foreach (var pair in pairs)
            {
                var similarity = SyntaxCompare.FindCommonSuperTree(pair.Left, pair.Right);
                if (similarity.Any())
                    Similarities.Add(similarity);
            }

            MatchData.InnerPairs = ins.Count();
            MatchData.OuterPairs = outs.Count();
            MatchData.Matches = Similarities.Count();
            MatchData.MatchNodeLengths = Similarities.Select(pair => pair.Count());
        }
    }
}
