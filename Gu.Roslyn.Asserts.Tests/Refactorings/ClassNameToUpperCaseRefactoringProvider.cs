namespace Gu.Roslyn.Asserts.Tests.Refactorings
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeRefactorings;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ClassNameToUpperCaseRefactoringProvider))]
    public class ClassNameToUpperCaseRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root.FindToken(context.Span.Start) is SyntaxToken token &&
                token.Parent is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.Identifier == token &&
                token.ValueText.Any(x => char.IsLower(x)))
            {
                context.RegisterRefactoring(
                    CodeAction.Create(
                        "To uppercase",
                        c => Task.FromResult(
                            context.Document.WithSyntaxRoot(
                                root.ReplaceToken(
                                    token,
                                    SyntaxFactory.Identifier(token.ValueText.ToUpper())))),
                        "To uppercase"));
            }
        }
    }
}
