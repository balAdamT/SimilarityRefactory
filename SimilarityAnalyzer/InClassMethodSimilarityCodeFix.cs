using System;
using System.Composition;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics.Where(d => d.Id == DiagnosticId))
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
