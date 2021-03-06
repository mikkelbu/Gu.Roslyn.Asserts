namespace Gu.Roslyn.Asserts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class AnalyzerAssert
    {
        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            var codeFix = new TCodeFix();
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.Create(expectedDiagnostic, new[] { codeWithErrorsIndicated }),
                new[] { fixedCode },
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, codeWithErrorsIndicated),
                MergeFixedCodeWithErrorsIndicated(codeWithErrorsIndicated, fixedCode),
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TAnalyzer, TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            FixAll(
                new TAnalyzer(),
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, codeWithErrorsIndicated),
                fixedCode,
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling the code.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, string codeWithErrorsIndicated, string fixedCode, IEnumerable<MetadataReference> metadataReferences = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.Create(expectedDiagnostic, new[] { codeWithErrorsIndicated }),
                MergeFixedCodeWithErrorsIndicated(new[] { codeWithErrorsIndicated }, fixedCode),
                SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling the code.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, IEnumerable<MetadataReference> metadataReferences = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.Create(expectedDiagnostic, codeWithErrorsIndicated),
                MergeFixedCodeWithErrorsIndicated(codeWithErrorsIndicated, fixedCode),
                SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling the code.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, IEnumerable<MetadataReference> metadataReferences = null, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.Create(expectedDiagnostic, codeWithErrorsIndicated),
                fixedCode,
                SuppressedDiagnostics,
                metadataReferences ?? MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, new[] { code }),
                new[] { fixedCode },
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                MergeFixedCode(code, fixedCode),
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TCodeFix>(ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode,
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, string code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                fix,
                DiagnosticsAndSources.Create(expectedDiagnostic, new[] { code }),
                new[] { fixedCode },
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                fix,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                MergeFixedCode(code, fixedCode),
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="code"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="fix">The <see cref="CodeFixProvider"/> to apply.</param>
        /// <param name="expectedDiagnostic">The expected diagnostic.</param>
        /// <param name="code">The code to analyze.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(CodeFixProvider fix, ExpectedDiagnostic expectedDiagnostic, IReadOnlyList<string> code, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            var analyzer = new PlaceholderAnalyzer(expectedDiagnostic.Id);
            FixAll(
                analyzer,
                fix,
                DiagnosticsAndSources.Create(expectedDiagnostic, code),
                fixedCode,
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, new[] { codeWithErrorsIndicated }),
                new[] { fixedCode },
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                MergeFixedCodeWithErrorsIndicated(codeWithErrorsIndicated, fixedCode),
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll<TAnalyzer, TCodeFix>(IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAll(
                analyzer,
                new TCodeFix(),
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode,
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                MergeFixedCodeWithErrorsIndicated(codeWithErrorsIndicated, fixedCode),
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode,
                SuppressedDiagnostics,
                MetadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="metadataReferences">The meta data references to use when compiling the code.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, IEnumerable<MetadataReference> metadataReferences, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
        {
            FixAll(
                analyzer,
                codeFix,
                DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated),
                fixedCode,
                SuppressedDiagnostics,
                metadataReferences,
                fixTitle,
                allowCompilationErrors);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="diagnosticsAndSources">The code and expected diagnostics.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="suppressedDiagnostics">The diagnostics to suppress when compiling.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAll(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, DiagnosticsAndSources diagnosticsAndSources, IReadOnlyList<string> fixedCode, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, suppressedDiagnostics, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, codeFix, sln, fixedCode, fixTitle, allowCompilationErrors);

            var fixAllProvider = codeFix.GetFixAllProvider();
            if (fixAllProvider != null)
            {
                foreach (var scope in fixAllProvider.GetSupportedFixAllScopes())
                {
                    FixAllByScope(analyzer, codeFix, sln, fixedCode, fixTitle, allowCompilationErrors, scope);
                }
            }
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllInDocument<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            FixAllByScope(
                    analyzer,
                    new TCodeFix(),
                    new[] { codeWithErrorsIndicated },
                    new[] { fixedCode },
                    fixTitle,
                    SuppressedDiagnostics,
                    MetadataReferences,
                    allowCompilationErrors,
                    FixAllScope.Document);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the code..</param>
        /// <param name="codeFix">The code fix to apply.</param>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="suppressedDiagnostics">The diagnostics to suppress when compiling.</param>
        /// <param name="metadataReferences">The meta data metadataReferences to add to the compilation.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        /// <param name="scope">The scope to apply fixes for.</param>
        public static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> codeWithErrorsIndicated, IReadOnlyList<string> fixedCode, string fixTitle, IEnumerable<string> suppressedDiagnostics, IEnumerable<MetadataReference> metadataReferences, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, suppressedDiagnostics, metadataReferences);
            var diagnostics = Analyze.GetDiagnostics(sln, analyzer);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllByScope(analyzer, codeFix, sln, fixedCode, fixTitle, allowCompilationErrors, scope);
        }

        /// <summary>
        /// Verifies that
        /// 1. <paramref name="codeWithErrorsIndicated"/> produces the expected diagnostics
        /// 2. The code fix fixes the code.
        /// </summary>
        /// <typeparam name="TAnalyzer">The type of the analyzer.</typeparam>
        /// <typeparam name="TCodeFix">The type of the code fix.</typeparam>
        /// <param name="codeWithErrorsIndicated">The code with error positions indicated.</param>
        /// <param name="fixedCode">The expected code produced by the code fix.</param>
        /// <param name="fixTitle">The title of the fix to apply if more than one.</param>
        /// <param name="allowCompilationErrors">If compilation errors are accepted in the fixed code.</param>
        public static void FixAllOneByOne<TAnalyzer, TCodeFix>(string codeWithErrorsIndicated, string fixedCode, string fixTitle = null, AllowCompilationErrors allowCompilationErrors = AllowCompilationErrors.No)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var analyzer = new TAnalyzer();
            var diagnosticsAndSources = DiagnosticsAndSources.CreateFromCodeWithErrorsIndicated(analyzer, codeWithErrorsIndicated);
            VerifyAnalyzerSupportsDiagnostics(analyzer, diagnosticsAndSources.ExpectedDiagnostics);
            var codeFix = new TCodeFix();
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var sln = CodeFactory.CreateSolution(diagnosticsAndSources, analyzer, SuppressedDiagnostics, MetadataReferences);
            var diagnostics = Analyze.GetDiagnostics(analyzer, sln);
            VerifyDiagnostics(diagnosticsAndSources, diagnostics);
            FixAllOneByOne(analyzer, codeFix, sln, new[] { fixedCode }, fixTitle, allowCompilationErrors);
        }

        private static void FixAllOneByOne(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, Solution solution, IReadOnlyList<string> fixedCode, string fixTitle, AllowCompilationErrors allowCompilationErrors)
        {
            var fixedSolution = Fix.ApplyAllFixableOneByOneAsync(solution, analyzer, codeFix, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(fixedCode, fixedSolution, "Applying fixes one by one failed.").GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private static async Task FixAllOneByOneAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> fixedCode, string fixTitle, AllowCompilationErrors allowCompilationErrors, Solution solution)
        {
            var fixedSolution = await Fix.ApplyAllFixableOneByOneAsync(solution, analyzer, codeFix, fixTitle, CancellationToken.None).ConfigureAwait(false);
            await AreEqualAsync(fixedCode, fixedSolution, "Applying fixes one by one failed.").ConfigureAwait(false);
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                await VerifyNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false);
            }
        }

        private static void FixAllByScope(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, Solution sln, IReadOnlyList<string> fixedCode, string fixTitle, AllowCompilationErrors allowCompilationErrors, FixAllScope scope)
        {
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var fixedSolution = Fix.ApplyAllFixableScopeByScopeAsync(sln, analyzer, codeFix, scope, fixTitle, CancellationToken.None).GetAwaiter().GetResult();
            AreEqualAsync(fixedCode, fixedSolution, $"Applying fixes for {scope} failed.").GetAwaiter().GetResult();
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                VerifyNoCompilerErrorsAsync(codeFix, fixedSolution).GetAwaiter().GetResult();
            }
        }

        private static async Task FixAllByScopeAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, IReadOnlyList<string> fixedCode, string fixTitle, AllowCompilationErrors allowCompilationErrors, Solution solution, FixAllScope scope)
        {
            VerifyCodeFixSupportsAnalyzer(analyzer, codeFix);
            var fixedSolution = await Fix.ApplyAllFixableScopeByScopeAsync(solution, analyzer, codeFix, scope, fixTitle, CancellationToken.None).ConfigureAwait(false);
            await AreEqualAsync(fixedCode, fixedSolution, $"Applying fixes for {scope} failed.").ConfigureAwait(false);
            if (allowCompilationErrors == AllowCompilationErrors.No)
            {
                await VerifyNoCompilerErrorsAsync(codeFix, fixedSolution).ConfigureAwait(false);
            }
        }

        private static List<string> MergeFixedCodeWithErrorsIndicated(IReadOnlyList<string> codeWithErrorsIndicated, string fixedCode)
        {
            var merged = new List<string>(codeWithErrorsIndicated.Count);
            var found = false;
            foreach (var code in codeWithErrorsIndicated)
            {
                if (code.IndexOf('↓') >= 0)
                {
                    if (found)
                    {
                        throw new AssertException("Expected only one with errors indicated.");
                    }

                    merged.Add(fixedCode);
                    found = true;
                }
                else
                {
                    merged.Add(code);
                }
            }

            if (!found)
            {
                throw new AssertException("Expected one with errors indicated.");
            }

            return merged;
        }

        private static List<string> MergeFixedCode(IReadOnlyList<string> codes, string fixedCode)
        {
            var merged = new List<string>(codes.Count);
            var found = false;
            var @namespace = CodeReader.Namespace(fixedCode);
            var fileName = CodeReader.FileName(fixedCode);
            foreach (var code in codes)
            {
                if (CodeReader.FileName(code) == fileName &&
                    CodeReader.Namespace(code) == @namespace)
                {
                    if (found)
                    {
                        throw new AssertException("Expected unique class names.");
                    }

                    merged.Add(fixedCode);
                    found = true;
                }
                else
                {
                    merged.Add(code);
                }
            }

            if (!found)
            {
                throw new AssertException("Failed merging expected one class to have same namespace and class name as fixedCode.\r\n" +
                                             "Try specifying a list with all fixed code.");
            }

            return merged;
        }
    }
}
