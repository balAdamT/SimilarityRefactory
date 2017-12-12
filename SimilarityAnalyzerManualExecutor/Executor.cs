using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SimilarityAnalyzer.Helpers;
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
    private bool list = false;

    NodeToNode identity = new NodeToNode();
    NodeToVector withVector = new NodeToVector(SyntaxMasks.AllNodes);
    ISyntaxComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation> superComparator = new SuperTreeComparator<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>();
    ISyntaxComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation> subComparator = new SemanticComparator<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>();
    ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector> subComparatorWithVector = new SemanticComparator<SyntaxPair<NodeWithVector>, NodeWithVector>();
    ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector> vectorComparator = new VectorComparator<SyntaxPair<NodeWithVector>, NodeWithVector>();

    public Executor(bool list)
    {
      this.list = list;
    }

    public void Execute(string path)
    {
      MSBuildWorkspace workspace = MSBuildWorkspace.Create();
      Solution solution = workspace.OpenSolutionAsync(path).Result;
      var projects = solution.Projects.Where(p => p.Name.EndsWith("-4.5")).ToList();

      Parallel.ForEach(projects, ExecuteForProject);

      Console.WriteLine("Done!");
      Console.ReadKey();
    }

    public void ExecuteForProject(string path)
    {
      MSBuildWorkspace workspace = MSBuildWorkspace.Create();
      Project project = workspace.OpenProjectAsync(path).Result;
      Console.WriteLine("Loaded project");
      while (true)
      {
        ExecuteForProject(project);
        Console.WriteLine("Done!");
        Console.ReadKey();
      }
    }

    private void ExecuteForProject(Project project){
      var projectName = project.Name;
      var compilation = project.GetCompilationAsync().Result;

      var m1 = Config1(compilation);
      Log(projectName, "sub", m1);
      var m2 = Config2(compilation);
      Log(projectName, "sup", m2);
      var m3 = Config3(compilation);
      Log(projectName, "vec", m3);
    }

    private void Log(string projectName, string configName, SimilarityMeasure measure)
    {
      Console.WriteLine($@"{projectName}:{configName.ToUpper()}:{measure}");
    }

    private SimilarityMeasure Config3(Compilation compilation)
    {
      var source = new MethodFragmentsInCompilation(compilation: compilation, minDepth: 8);
      var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeWithVector>, NodeWithVector>(source, withVector, new List<ISyntaxComparator<SyntaxPair<NodeWithVector>, NodeWithVector>>() { vectorComparator, subComparatorWithVector });
      var similarities = analyzer.FindAll().ToList();

      if (list)
        ListSimilarities(similarities);

      return analyzer.Measure;
    }

    private SimilarityMeasure Config2(Compilation compilation)
    {
      var source = new LeavesInCompilation(compilation);
      var analyzer = new MeasuredSimilarityFinder<SyntaxLeafPair<NodeAsRepresentation>, NodeAsRepresentation>(source, identity, Enumerable.Repeat(superComparator, 1).ToList());
      var similarities = analyzer.FindAll().ToList();

      if (list)
        ListSimilarities(similarities);

      return analyzer.Measure;
    }

    private SimilarityMeasure Config1(Compilation compilation)
    {
      var source = new MethodFragmentsInCompilation(compilation, 8);
      var analyzer = new MeasuredSimilarityFinder<SyntaxPair<NodeAsRepresentation>, NodeAsRepresentation>(source, identity, Enumerable.Repeat(subComparator, 1).ToList());
      var similarities = analyzer.FindAll().ToList();

      if (list)
        ListSimilarities(similarities);

      return analyzer.Measure;
    }

    private void ListSimilarities(List<SyntaxPair<NodeAsRepresentation>> similarities)
    {
      foreach (var similiarity in similarities)
      {
        Console.WriteLine(
          $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
        );
      }
    }
    private void ListSimilarities(List<SyntaxLeafPair<NodeAsRepresentation>> similarities)
    {
      foreach (var similiarity in similarities)
      {
        Console.WriteLine(
            $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
        );
      }
    }


    private void ListSimilarities(List<SyntaxPair<NodeWithVector>> similarities)
    {
      foreach (var similiarity in similarities)
      {
        Console.WriteLine(
            $@"{similiarity.Left.Node.GetLocation().GetLineSpan()} vs {similiarity.Right.Node.GetLocation().GetLineSpan()}"
        );
      }
    }


  }

}
