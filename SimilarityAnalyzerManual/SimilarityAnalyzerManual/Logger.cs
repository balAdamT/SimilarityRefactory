using SimilarityAnalyzer.SimilarityTree.SuperTree;
using SimilarityAnalyzer.SyntaxComparision.Algorithm;
using SimilarityAnalyzer.SyntaxComparision.Data;
using SimilarityAnalyzer.SyntaxVectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimilarityAnalyzerManualExecutor
{
    internal class Logger
    {
        private DateTime currentTimeStamp;
        private string currentKey;
        private string currentProjectName;
        private string header;

        public Logger(string target, int minDepth)
        {
            header = $"Target: {target}{Environment.NewLine}Minimum depth:{minDepth}{Environment.NewLine}";
        }

        internal void SetCompletionDate(DateTime now)
        {
            currentTimeStamp = now;
        }

        internal void SetCurrentKey(string key)
        {
            currentKey = key;
        }

        internal void SetProjectName(string projectName)
        {
            currentProjectName = projectName;
        }

        internal void LogMeasures(SimilarityMeasure measure)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"{currentKey.ToUpper()}:{measure}");

            Log("measures", sb);
        }

        internal void LogSimilarities(List<SyntaxPair<NodeAsRepresentation>> similarities)
        {
            var sb = new StringBuilder();

            foreach (var similiarity in similarities)
            {
                sb.AppendLine(
                  $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
                );
            }

            Log("similarities", sb);
        }
        internal void LogSimilarities(List<SyntaxLeafPair<NodeAsRepresentation>> similarities)
        {
            var sb = new StringBuilder();

            foreach (var similiarity in similarities)
            {
                sb.AppendLine(
                    $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
                );
            }

            Log("similarities", sb);
        }


        internal void LogSimilarities(List<SyntaxPair<NodeWithVector>> similarities)
        {
            var sb = new StringBuilder();

            foreach (var similiarity in similarities)
            {
                sb.AppendLine(
                    $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
                );
            }

            Log("similarities", sb);
        }

        private void Log(string resultType, StringBuilder sb)
        {
            var currentTimeStampString = currentTimeStamp.ToString("yyyyMMdd");
            var fileName = currentTimeStampString + "_" + currentKey + "_" + resultType + "_for_" + currentProjectName + ".txt";
            var content = GetHeader() + Environment.NewLine + sb.ToString();

            File.WriteAllText(fileName, content);
            Console.WriteLine("Wrote {0} results to {1}!", resultType, fileName);
        }

        private string GetHeader()
        {
            return header + Environment.NewLine + $"Config: {currentKey}";
        }
    }
}