using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimilarityAnalyzer.Logic
{
    static class FragmentExplorer
    {
        public static void ForEachFragment(SyntaxNode parent, Action<SyntaxNode> action)
        {
            action.Invoke(parent);

            foreach(var child in parent.DescendantNodes())
            {
                ForEachFragment(child, action);
            }
        }
    }
}
