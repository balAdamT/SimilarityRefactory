using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimilarityAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InClassMethodSimilarityCodeFix)), Shared]
    public class InClassMethodSimilarityCodeFix : CodeFixProvider
    {
        public const string DiagnosticId = InClassMethodSimilarityAnalyzer.DiagnosticId;
        public const string Title = "Extract methods";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => d.Id == DiagnosticId))
            {
                context.RegisterCodeFix(CodeAction.Create(Title, c => FixIt(context.Document, c), equivalenceKey: Title), diagnostic);
            }
        }

        private Task<Document> FixIt(Document document, CancellationToken c)
        {
            return null;
        }
    }

}
