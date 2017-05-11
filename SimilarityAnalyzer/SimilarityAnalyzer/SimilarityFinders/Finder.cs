using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.SimilarityFinders
{
    public abstract class Finder
    {
        protected ClassDeclarationSyntax @class;

        public List<IEnumerable<NodePair>> Similarities { get; protected set; } = new List<IEnumerable<NodePair>>();

        public SimilarityData MatchData { get; protected set; } = new SimilarityData();

        public Finder(ClassDeclarationSyntax @class)
        {
            this.@class = @class;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            FindSimilarities();
            Similarities = Similarities.Where(similarity => similarity.Count() > 1).ToList();
            watch.Stop();
            MatchData.RunTimeInMs = watch.ElapsedMilliseconds;
        }

        protected abstract void FindSimilarities();
    }
}
