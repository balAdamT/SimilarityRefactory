using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.Data;
using SimilarityAnalyzer.Descriptor;
using SimilarityAnalyzer.Logic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityFinders
{
    public class CommonHashFinder : Finder
    {
        public CommonHashFinder(ClassDeclarationSyntax @class) : base(@class)
        {
        }

        protected override void FindSimilarities()
        {
            var classMap = new Multimap<SyntaxNodeManager, NodePairManager>();

            foreach (MethodDeclarationSyntax method
                in @class.Members.Where(member => member.Kind() == SyntaxKind.MethodDeclaration))
            {
                TreeExplorer.ForEachLeaf(method, fragment => classMap.AddFragmentOfMethod(new SyntaxNodeManager(method), new SyntaxNodeManager(fragment)));
            }

            var outs = classMap.GetOuterPairs();
            var ins = classMap.GetInnerPairs();
            var pairs = outs.Concat(ins);

            foreach (var pair in pairs)
            {
                if (pair.DescriptionMatches())
                {
                    if (pair.EqualPair())
                    Similarities.Add(Enumerable.Repeat(pair.ToNodePair(), 1));
                }
            }

            MatchData.InnerPairs = ins.Count();
            MatchData.OuterPairs = outs.Count();
            MatchData.Matches = Similarities.Count();
            MatchData.MatchNodeLengths = Similarities.Select(pair => pair.Count());
            MatchData.MatchSpans = Similarities.Select(pair => pair.Last().SpainPair);
        }
    }
}
