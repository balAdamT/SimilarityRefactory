using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxVectors.Masking
{
    internal class AllNodesMask : ISyntaxMask
    {
        public bool Skip(SyntaxKind kind)
        {
            return false;
        }

        public bool Terminal(SyntaxKind kind)
        {
            return false;
        }
    }
}