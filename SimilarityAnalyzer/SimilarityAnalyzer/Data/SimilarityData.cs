using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Data
{
    public class SimilarityData
    {
        public int OuterPairs { get; set; } = 0;

        public int InnerPairs { get; set; } = 0;

        public int Matches { get; set; } = 0;

        public IEnumerable<int> MatchNodeLengths { get; set; }

        public IEnumerable<Tuple<TextSpan,TextSpan>> MatchSpans { get; set; }

        public long RunTimeInMs { get; set; } = 0;
    }
}
