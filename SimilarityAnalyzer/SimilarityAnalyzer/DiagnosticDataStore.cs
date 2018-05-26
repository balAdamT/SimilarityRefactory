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
    public class DiagnosticDataStore
    {
        private static readonly DiagnosticDataStore instance = new DiagnosticDataStore();
        public static DiagnosticDataStore Instance => instance;

        private Dictionary<string, object> store = new Dictionary<string, object>();

        public void Store(string diagnosticId, object data)
        {
            store[diagnosticId] = data;
        }

        public object Retrieve(string diagnosticId)
        {
            return store[diagnosticId];
        }
    }
}
