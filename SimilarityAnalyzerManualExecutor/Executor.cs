using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SimilarityTreeExplorer.SubTree;
using SimilarityTreeExplorer.SuperTree;
using SyntaxComparision.Algorithm;
using SyntaxComparision.Data;
using SyntaxComparision.Interfaces;
using SyntaxVectors;
using SyntaxVectors.Masking;

namespace SimilarityAnalyzerManualExecutor
{
  internal class Executor
  {
    NodeToNode identity = new NodeToNode();
    NodeToVector withVector = new NodeToVector(SyntaxMasks.AllNodes);
    ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation> superComparator = (ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>)new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>();
    ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation> subComparator = (ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>)new SemanticComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>();
    ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector> subComparatorWithVector = (ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector>)new SemanticComparator<SyntaxPair<NodeWithVector>, NodeWithVector>();
    ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector> vectorComparator = (ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector>)new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector>();

    public void Execute(string path)
    {
      MSBuildWorkspace workspace = MSBuildWorkspace.Create();
      Solution solution = workspace.OpenSolutionAsync(path).Result;
      var projects = solution.Projects.Where(p => p.Name.EndsWith("-4.5"));


      Parallel.ForEach(projects, project =>
      {
        var projectName = project.Name;
        var compilation = project.GetCompilationAsync().Result;

        var m1 = Config1(compilation);
        Log(projectName, "sub", m1);
        var m2 = Config2(compilation);
        Log(projectName, "sup", m2);
        var m3 = Config3(compilation);
        Log(projectName, "vec", m3);
      });

      Console.WriteLine("Done!");
      Console.ReadKey();
    }

    private void Log(string projectName, string configName, SimilarityMeasure measure)
    {
      Console.WriteLine($@"{projectName}:{configName.ToUpper()}:{measure}");
    }

    private SimilarityMeasure Config3(Compilation compilation)
    {
      var source = new MethodFragmentsInCompilation(compilation);
      var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector>(source, withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector>>() {vectorComparator, subComparatorWithVector });
      var similarities = analyzer.FindAll().ToList();
      return analyzer.Measure;
    }

    private SimilarityMeasure Config2(Compilation compilation)
    {
      var source = new LeavesInCompilation(compilation);
      var analyzer = new MeasuredSimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>(source, identity, Enumerable.Repeat(superComparator, 1).ToList());
      var similarities = analyzer.FindAll().ToList();
      return analyzer.Measure;
    }

    private SimilarityMeasure Config1(Compilation compilation)
    {
      var source = new MethodFragmentsInCompilation(compilation);
      var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>(source, identity, Enumerable.Repeat(subComparator, 1).ToList());
      var similarities = analyzer.FindAll().ToList();
      return analyzer.Measure;
    }
  }
}
