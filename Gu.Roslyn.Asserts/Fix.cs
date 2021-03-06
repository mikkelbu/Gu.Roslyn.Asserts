namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Helper class for applying code fixes.
    /// </summary>
    public static class Fix
    {
        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one. If only one pass null.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        public static Solution Apply(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic, string fixTitle = null)
        {
            var actions = GetActionsAsync(solution, codeFix, diagnostic).GetAwaiter().GetResult();
            var action = FindAction(actions, fixTitle);
            var operations = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
            if (operations.TrySingleOfType(out ApplyChangesOperation operation))
            {
                return operation.ChangedSolution;
            }

            throw new InvalidOperationException($"Expected one operation, was {string.Join(", ", operations)}");
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostics">The diagnostics.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one. If only one pass null.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        public static Solution Apply(Solution solution, CodeFixProvider codeFix, IReadOnlyList<ImmutableArray<Diagnostic>> diagnostics, string fixTitle = null)
        {
            var flatDiagnostics = diagnostics.SelectMany(x => x).ToArray();
            if (flatDiagnostics.Length == 1)
            {
                return Apply(solution, codeFix, flatDiagnostics[0], fixTitle);
            }

            var trees = flatDiagnostics.Select(x => x.Location.SourceTree).Distinct().ToArray();
            if (trees.Length == 1)
            {
                var document = solution.Projects.SelectMany(x => x.Documents)
                                       .Single(x => x.GetSyntaxTreeAsync().GetAwaiter().GetResult() == trees[0]);
                var provider = TestDiagnosticProvider.CreateAsync(solution, codeFix, fixTitle, flatDiagnostics).GetAwaiter().GetResult();
                var context = new FixAllContext(document, codeFix, FixAllScope.Document, provider.EquivalenceKey, flatDiagnostics.Select(x => x.Id), provider, CancellationToken.None);
                var action = WellKnownFixAllProviders.BatchFixer.GetFixAsync(context).GetAwaiter().GetResult();
                var operations = action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
                if (operations.TrySingleOfType(out ApplyChangesOperation operation))
                {
                    return operation.ChangedSolution;
                }
            }

            throw new InvalidOperationException($"Failed applying fix, bug in Gu.Roslyn.Asserts");
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one. If only one pass null.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        public static async Task<Solution> ApplyAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic, string fixTitle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var actions = await GetActionsAsync(solution, codeFix, diagnostic).ConfigureAwait(false);
            var action = FindAction(actions, fixTitle);
            var operations = await action.GetOperationsAsync(cancellationToken)
                                         .ConfigureAwait(false);
            if (operations.TrySingleOfType(out ApplyChangesOperation operation))
            {
                return operation.ChangedSolution;
            }

            throw new InvalidOperationException($"Expected one operation, was {string.Join(", ", operations)}");
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableOneByOneAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, string fixTitle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, codeFix).ConfigureAwait(false);
            fixable = fixable.OrderBy(x => x.Location, LocationComparer.BySourceSpan).ToArray();
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                fixedSolution = await ApplyAsync(fixedSolution, codeFix, fixable[0], fixTitle, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, codeFix).ConfigureAwait(false);
                fixable = fixable.OrderBy(x => x.Location, LocationComparer.BySourceSpan).ToArray();
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix one fix at the time until it stops fixing the code.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAllFixableScopeByScopeAsync(Solution solution, DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, FixAllScope scope, string fixTitle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fixable = await Analyze.GetFixableDiagnosticsAsync(solution, analyzer, codeFix).ConfigureAwait(false);
            var fixedSolution = solution;
            int count;
            do
            {
                count = fixable.Count;
                if (count == 0)
                {
                    return fixedSolution;
                }

                var diagnosticProvider = await TestDiagnosticProvider.CreateAsync(fixedSolution, codeFix, fixTitle, fixable).ConfigureAwait(false);
                fixedSolution = await ApplyAsync(codeFix, scope, diagnosticProvider, cancellationToken).ConfigureAwait(false);
                fixable = await Analyze.GetFixableDiagnosticsAsync(fixedSolution, analyzer, codeFix).ConfigureAwait(false);
            }
            while (fixable.Count < count);
            return fixedSolution;
        }

        /// <summary>
        /// Fix the solution by applying the code fix.
        /// </summary>
        /// <returns>The fixed solution or the same instance if no fix.</returns>
        internal static async Task<Solution> ApplyAsync(CodeFixProvider codeFix, FixAllScope scope, TestDiagnosticProvider diagnosticProvider, CancellationToken cancellationToken)
        {
            var context = new FixAllContext(
                diagnosticProvider.Document,
                codeFix,
                scope,
                diagnosticProvider.EquivalenceKey,
                codeFix.FixableDiagnosticIds,
                diagnosticProvider,
                cancellationToken);
            var action = await codeFix.GetFixAllProvider().GetFixAsync(context).ConfigureAwait(false);

            var operations = await action.GetOperationsAsync(cancellationToken)
                                         .ConfigureAwait(false);
            if (operations.TrySingleOfType(out ApplyChangesOperation operation))
            {
                return operation.ChangedSolution;
            }

            throw new InvalidOperationException($"Expected one operation, was {string.Join(", ", operations)}");
        }

        /// <summary>
        /// Get the code actions registered by <paramref name="codeFix"/> for <paramref name="solution"/>.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <returns>The list of registered actions.</returns>
        internal static IReadOnlyList<CodeAction> GetActions(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic)
        {
            var document = solution.GetDocument(diagnostic.Location.SourceTree);
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, d) => actions.Add(a),
                CancellationToken.None);
            codeFix.RegisterCodeFixesAsync(context).GetAwaiter().GetResult();
            return actions;
        }

        /// <summary>
        /// Get the code actions registered by <paramref name="codeFix"/> for <paramref name="solution"/>.
        /// </summary>
        /// <param name="solution">The solution with the diagnostic.</param>
        /// <param name="codeFix">The code fix.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <returns>The list of registered actions.</returns>
        internal static async Task<IReadOnlyList<CodeAction>> GetActionsAsync(Solution solution, CodeFixProvider codeFix, Diagnostic diagnostic)
        {
            var document = solution.GetDocument(diagnostic.Location.SourceTree);
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(
                document,
                diagnostic,
                (a, d) => actions.Add(a),
                CancellationToken.None);
            await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
            return actions;
        }

        private static CodeAction FindAction(IReadOnlyList<CodeAction> actions, string fixTitle)
        {
            if (fixTitle == null)
            {
                if (actions.TrySingle(out var action))
                {
                    return action;
                }

                if (actions.Count == 0)
                {
                    throw new AssertException("Expected one code fix, was 0.");
                }

                throw new AssertException($"Expected only one code fix, found {actions.Count}:\r\n" +
                                             $"{string.Join("\r\n", actions.Select(x => x.Title))}\r\n" +
                                             "Use the overload that specifies title.");
            }
            else
            {
                if (actions.TrySingle(x => x.Title == fixTitle, out var action))
                {
                    return action;
                }

                if (actions.All(x => x.Title != fixTitle))
                {
                    var errorBuilder = StringBuilderPool.Borrow();
                    errorBuilder.AppendLine($"Did not find a code fix with title {fixTitle}.").AppendLine("Found:");
                    foreach (var codeAction in actions)
                    {
                        errorBuilder.AppendLine(codeAction.Title);
                    }

                    throw new AssertException(StringBuilderPool.Return(errorBuilder));
                }

                if (actions.Count(x => x.Title == fixTitle) == 0)
                {
                    throw new AssertException("Expected one code fix, was 0.");
                }

                throw new AssertException($"Expected only one code fix, found {actions.Count}:\r\n" +
                                             $"{string.Join("\r\n", actions.Select(x => x.Title))}\r\n" +
                                             "Use the overload that specifies title.");
            }
        }

        /// <inheritdoc />
        internal sealed class TestDiagnosticProvider : FixAllContext.DiagnosticProvider
        {
            private readonly IReadOnlyList<Diagnostic> diagnostics;

            private TestDiagnosticProvider(IReadOnlyList<Diagnostic> diagnostics, Document document, string equivalenceKey)
            {
                this.diagnostics = diagnostics;
                this.Document = document;
                this.EquivalenceKey = equivalenceKey;
            }

            /// <summary>
            /// Gets the document from the first diagnostic.
            /// </summary>
            public Document Document { get; }

            /// <summary>
            /// Gets the equivalence key for the first registered action.
            /// </summary>
            public string EquivalenceKey { get; }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                return Task.FromResult((IEnumerable<Diagnostic>)this.diagnostics);
            }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.diagnostics.Where(i => i.Location.GetLineSpan().Path == document.Name));
            }

            /// <inheritdoc />
            public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.diagnostics.Where(i => !i.Location.IsInSource));
            }

            /// <summary>
            /// Create an instance of <see cref="TestDiagnosticProvider"/>.
            /// </summary>
            /// <returns>The <see cref="TestDiagnosticProvider"/>.</returns>
            internal static async Task<TestDiagnosticProvider> CreateAsync(Solution solution, CodeFixProvider codeFix, string fixTitle, IReadOnlyList<Diagnostic> diagnostics)
            {
                var actions = new List<CodeAction>();
                var diagnostic = diagnostics.First();
                var context = new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => actions.Add(a), CancellationToken.None);
                await codeFix.RegisterCodeFixesAsync(context).ConfigureAwait(false);
                var action = FindAction(actions, fixTitle);
                return new TestDiagnosticProvider(diagnostics, solution.GetDocument(diagnostics.First().Location.SourceTree), action.EquivalenceKey);
            }
        }
    }
}
