using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimilarityAnalyzer.SyntaxComparision.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SimilarityAnalyzer.SimilarityTree.SubTree
{
    public class DataflowComparator<TPair, TRepresentation, TInformation> : ISyntaxComparator<TPair, TRepresentation, TInformation>
      where TRepresentation : ISyntaxRepresentation
      where TPair : ISyntaxPair<TRepresentation>
      where TInformation : ISyntaxInformation
    {
        public bool SyntaxEquals(TPair pair, TInformation information)
        {
            return Equals(pair.Left.Node, pair.Right.Node, new Dictionary<string, string>());
        }

        private bool Equals(SyntaxNode left, SyntaxNode right, Dictionary<string, string> variables)
        {
            if (left.Kind() != right.Kind())
                return false;

            if (left.Kind() == SyntaxKind.VariableDeclarator)
            {
                if (VariableEquals((VariableDeclaratorSyntax)left, (VariableDeclaratorSyntax)right, variables))
                    return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), variables);
            }

            if (left.Kind() != SyntaxKind.IdentifierName)
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), variables);

            //This WILL update dictionary for ALL recursion paths but that is okay - dictionary must be globally integral
            if (IdentifierEquals((IdentifierNameSyntax)left, (IdentifierNameSyntax)right, variables))
                return ChildrenEquals(left.ChildNodes(), right.ChildNodes(), variables);

            return false;
        }

        private bool VariableEquals(VariableDeclaratorSyntax left, VariableDeclaratorSyntax right, Dictionary<string, string> variables)
        {
            string leftTruth = left.Identifier.Text;
            string rightTruth = right.Identifier.Text;

            variables.TryGetValue(leftTruth, out string rightGuess);

            //We have seen this before, so it must have a partner
            if (rightGuess != null)
                return rightTruth == rightGuess;

            //First occurence of these variables, we can accept it
            variables[leftTruth] = rightTruth;
            return true;
        }

        private bool IdentifierEquals(IdentifierNameSyntax left, IdentifierNameSyntax right, Dictionary<string, string> variables)
        {
            string leftTruth = left.Identifier.Text;
            string rightTruth = right.Identifier.Text;

            variables.TryGetValue(leftTruth, out string rightGuess);

            //We have seen this before, so it must have a partner
            if (rightGuess != null)
                return rightTruth == rightGuess;

            //First occurence of these variables, we can accept it
            variables[leftTruth] = rightTruth;
            return true;
        }

        private bool ChildrenEquals(IEnumerable<SyntaxNode> leftChildren, IEnumerable<SyntaxNode> rightChildren, Dictionary<string, string> variables)
        {
            var parallelEnumerator = leftChildren.Zip(rightChildren, (x, y) => new { Left = x, Right = y });

            //var result = true;
            //foreach (var pair in parallelEnumerator)
            //{
            //  if (!Equals(pair.Left, pair.Right))
            //    result = false;
            //}

            if (parallelEnumerator.Any(pair => !Equals(pair.Left, pair.Right, variables)))
                return false;

            //No inequalities, check if children are balanced
            return leftChildren.Count() == rightChildren.Count();
        }
    }
}

